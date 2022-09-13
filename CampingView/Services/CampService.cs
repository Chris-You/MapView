using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using CampingView.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace CampingView.Services
{

    public interface ICampService
    {
        CampResModel GetCampList(CampReqModel req);
        CampImgResModel GetCampImgList(CampReqModel req);

        CampResModel GetCampSearch(string search);
    }

    public class CampService : ICampService
    {

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public CampService(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }



        private bool IsCampFileCheck(string path)
        {
            //todo 한달마다 파일을 갱신한다.

            // 캠프 리스트카 파일에 존재하는지 체크
            if (!File.Exists(path))
            {
                // 없으면 API 호출 후  json 파일 생성
                return false;
            }
            else
            {
                // 있으면 파일을 읽어들여 로드 (API 호출을 최소화)
                return true;
            }
        }


        public CampResModel CampList(CampReqModel req)
        {
            var path = _hostingEnvironment.WebRootPath + "/" + _configuration.GetSection("CAMP:CAMP_LIST_JSON").Value;
            path = path.Replace("{}", req.pageNo.ToString());

            //var model = new CampResModel();

            string url = req.searchurl; // URL
            url += "?ServiceKey=" + req.serviceKey; // Service Key
            url += "&pageNo=" + req.pageNo;
            url += "&numOfRows=" + req.numOfRows;
            url += "&MobileOS=" + req.MobileOS;
            url += "&MobileApp=" + req.MobileApp;
            url += "&_type=json";

            if (req.radius > 0)
            {
                url += "&mapX=" + req.mapX;
                url += "&mapY=" + req.mapY;
                url += "&radius=" + req.radius;
            }


            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            string results = string.Empty;
            HttpWebResponse response;
            using (response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                results = reader.ReadToEnd();

                // 응답데이터를 json 파일로 생성

                using (FileStream fs = File.Create(path))
                {
                    Byte[] json = new UTF8Encoding(true).GetBytes(results);
                    fs.Write(json, 0, json.Length);
                }

                return JsonConvert.DeserializeObject<CampResModel>(results);
                /*
                var list = model.response.body.items.item.Where(w => w.facltDivNm == "지자체").ToList();

                this.ManageCamp(model.response.body.items, list);

                model.response.body.items.item = list;
                */

                
            }
        }

        public CampResModel GetCampList(CampReqModel req)
        {
            var model = new CampResModel();
            List<List<CampItem>> arr = new List<List<CampItem>>();

            var path = _hostingEnvironment.WebRootPath + "/" + _configuration.GetSection("CAMP:CAMP_LIST_JSON").Value;
            path = path.Replace("{}", req.pageNo.ToString());

            if (IsCampFileCheck(path) == false)
            {
                var res = this.CampList(req);

                var cnt = res.response.body.totalCount / res.response.body.numOfRows;
                var mod = (res.response.body.totalCount % res.response.body.numOfRows > 0) ? cnt++ : cnt;
                for (var i=1; i<= cnt; i++)
                {
                    req.pageNo++;
                    var res2 = this.CampList(req);
                }
            }
            else
            {
                var dir = _hostingEnvironment.WebRootPath + "/" + _configuration.GetSection("CAMP:CAMP_LIST_DIR").Value;

                var di = new DirectoryInfo(dir);
                foreach(var file in di.GetFiles())
                {
                    var str = File.ReadAllText(file.FullName);
                    var resp = JsonConvert.DeserializeObject<CampResModel>(str);
                    
                    if (resp.response.body.items != null && resp.response.body.items.item.Count() > 0)
                    {
                        arr.Add(resp.response.body.items.item.Where(w => w.facltDivNm == "지자체").ToList());
                    }

                }
                var item = new List<CampItem>();
                foreach (var li in arr)
                {
                    item = item.Union(li).ToList();
                }
                Random rand = new Random();
                var shuffled = item.OrderBy(_ => rand.Next()).ToList();


                model.response.header.resultCode = "00";
                model.response.header.resultMsg = "OK";
                model.response.body.items.item = shuffled;
                model.response.body.numOfRows = item.Count();
                model.response.body.totalCount = item.Count();
                model.response.body.pageNo = 1;


                /*
                var jsonStr = File.ReadAllText(path);
                model = JsonConvert.DeserializeObject<CampResModel>(jsonStr);
                var list = model.response.body.items.item.Where(w => w.facltDivNm == "지자체").ToList();

                this.ManageCamp(model.response.body.items, list);

                model.response.body.items.item = list;
                */
            }

            return model;
        }

        public void ManageCamp(CampItems items, List<CampItem> list)
        {
            var allowCamp = _configuration.GetSection("CAMP:ALLOW_CAMP").Value;
            var denyCamp = _configuration.GetSection("CAMP:DENY_CAMP").Value;


            if (string.IsNullOrEmpty(allowCamp) == false)
            {
                var allow = allowCamp.Split(',');

                foreach (var i in allow)
                {
                    var data = items.item.Where(w => w.contentId == i.Trim()).FirstOrDefault();

                    if (data != null) list.Add(data);
                }
            }

            if (string.IsNullOrEmpty(denyCamp) == false)
            {
                var deny = denyCamp.Split(',');


                list.RemoveAll(x => deny.Contains(x.contentId));
                /*
                foreach (var i in deny)
                {
                    var bl = list.Remove(new CampItem() { contentId = i });
                }
                */
            }
        }


        public CampImgResModel GetCampImgList(CampReqModel req)
        {
            var model = new CampImgResModel();

            // 이미지 경로를 조회
            //var root = _hostingEnvironment.WebRootPath;
            var pathOrigin = _hostingEnvironment.WebRootPath + "/" + _configuration.GetSection("CAMP:CAMP_IMAGE_BASE_PATH").Value + "/" + req.contentId;
            var pathThum = pathOrigin + "/" + _configuration.GetSection("CAMP:CAMP_IMAGE_THUM").Value;
            var imgUrl = "/" + _configuration.GetSection("CAMP:CAMP_IMAGE_BASE_PATH").Value + "/" + req.contentId + "/" + _configuration.GetSection("CAMP:CAMP_IMAGE_THUM").Value;

            if (this.ExistImageFile(pathThum) == false)
            {
                // 이미지 폴더가 없거나, 이미지가 없으면 썸네일 파일 생성
                DirectoryInfo di = Directory.CreateDirectory(pathThum);

                string url = req.searchurl; // URL
                url += "?ServiceKey=" + req.serviceKey; // Service Key
                url += "&MobileOS=" + req.MobileOS;
                url += "&MobileApp=" + req.MobileApp;
                url += "&contentId=" + req.contentId;
                url += "&_type=json";

                var request = (HttpWebRequest)WebRequest.Create(url);
                request.Method = "GET";

                string results = string.Empty;
                HttpWebResponse response;
                using (response = request.GetResponse() as HttpWebResponse)
                {
                    StreamReader reader = new StreamReader(response.GetResponseStream());
                    results = reader.ReadToEnd();

                    model = JsonConvert.DeserializeObject<CampImgResModel>(results);
                }

                if (model.response.body.items != null)
                {
                    if (model.response.body.items.item.Count() > 0)
                    {
                        // 파일 다운로드
                        foreach (var f in model.response.body.items.item)
                        {
                            var name = f.imageUrl.Split('/').LastOrDefault();
                            var targetFile = pathOrigin + "/" + name;

                            this.ImageDownload(f.imageUrl, targetFile);
                        }

                        this.ImageResize(pathOrigin, pathThum);
                    }
                }
            }


            // 컨텐츠 썸네일 이미지 폴더에 이미지가 있으면 해당 파일 경로로 노출
            DirectoryInfo di2 = new DirectoryInfo(pathThum);
            var files = di2.EnumerateFiles();

            model.response = new CampImgResponse();
            model.response.body = new CampImgBody();
            model.response.body.items = new CampImgItems();
            model.response.body.items.item = new List<CampImgItem>();

            model.response.header = new CampImgHeader();
            model.response.header.resultCode = "0000";
            model.response.header.resultMsg = "OK";

            model.response.body.pageNo = 1;
            model.response.body.totalCount = files.Count();
            model.response.body.numOfRows = files.Count();

            var i = 1;
            foreach (var file in files)
            {
                var item = new CampImgItem();
                item.contentId = req.contentId;
                item.imageUrl = imgUrl + "/" + file.Name;
                item.imageUrlOri = "/" + _configuration.GetSection("CAMP:CAMP_IMAGE_BASE_PATH").Value + "/" + req.contentId + "/" + file.Name.Replace("thum_", "");
                item.serialnum = Convert.ToString(i++);

                model.response.body.items.item.Add(item);
            }


            return model;

        }

        private bool ExistImageFile(string path)
        {
            if (Directory.Exists(path) == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        private void ImageDownload(string url, string targetOrigin)
        {
            using (WebClient webClient = new WebClient())
            {
                webClient.DownloadFile(url, targetOrigin);
            }
        }

        private void ImageResize(string pathOrigin, string pathThum)
        {

            DirectoryInfo di = new DirectoryInfo(pathOrigin);

            var files = di.EnumerateFiles();


            foreach (var file in files)
            {
                using var image = Image.Load(file.OpenRead());
                image.Mutate(x => x.Resize(600, 400));

                image.Save(pathThum + "/thum_" + file.Name);

            }


        }


        public CampResModel GetCampSearch(string search)
        {
            var model = new CampResModel();

            List<List<CampItem>> arr = new List<List<CampItem>>();

            var dir = _hostingEnvironment.WebRootPath + "/" + _configuration.GetSection("CAMP:CAMP_LIST_DIR").Value;

            var di = new DirectoryInfo(dir);
            foreach (var file in di.GetFiles())
            {
                var str = File.ReadAllText(file.FullName);
                var resp = JsonConvert.DeserializeObject<CampResModel>(str);

                if (resp.response.body.items != null && resp.response.body.items.item.Count() > 0)
                {
                    arr.Add(resp.response.body.items.item.Where(w => w.facltNm.IndexOf(search) >= 0).ToList());
                }
            }

            var item = new List<CampItem>();
            foreach (var li in arr)
            {
                item = item.Union(li).ToList();
            }

            model.response.header.resultCode = "00";
            model.response.header.resultMsg = "OK";
            model.response.body.items.item = item;
            model.response.body.numOfRows = item.Count();
            model.response.body.totalCount = item.Count();
            model.response.body.pageNo = 1;

            return model;
        }
    }
}
