using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using CampView.Models.Charger;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using CampView.Util;
using StackExchange.Redis;
using Microsoft.Extensions.Options;
using CampView.Models.CustomSettings;
using System.Reflection;

namespace CampView.Services
{

    public interface IChargerService
    {
        List<ChargerModel> GetChargerList(ChargerReqModel req);
        List<ChargerItem> GetChargerAPIList(ChargerReqModel req);
        List<ChargerStatusItem> GetChargerStatusAPIList(ChargerReqModel req);


        string GetZcode(ChargerReqModel req);
        string GetZScode(ChargerReqModel req);

        string GetCodeNm(CodeType codeType, string code);
    }

    public class ChargerService : IChargerService
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IOptions<ChargerCode> _chargerConfig;


        public ChargerService(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, IHttpClientFactory clientFactory,
                             IOptions<ChargerCode> chargerConfig)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _clientFactory = clientFactory;
            _chargerConfig = chargerConfig;

        }




        public List<ChargerModel> GetChargerList(ChargerReqModel req)
        {

            var path = _hostingEnvironment.WebRootPath + "/" + _configuration.GetSection("CHARGER:CHARGER_LIST_JSON").Value;
            path = path.Replace("{}", req.zcode.ToString());

            if (this.ExistFile(path) == false)
            {
                var res = this.ChargerAPI(req,0);
                var itemList = res.items.item;

                var cnt = res.totalCount / res.numOfRows;
                var mod = (res.totalCount % res.numOfRows > 0) ? cnt++ : cnt;

                Parallel.For(0, cnt, (i) =>
                {
                    ++req.pageNo;
                    var res2 = this.ChargerAPI(req, 0);

                    itemList = itemList.Union(res2.items.item).ToList();
                });


                var list = itemList.Select(s => new {
                    s.statNm
                   , s.statId
                   , s.addr
                   , s.location
                   , s.lat
                   , s.lng
                   , s.zcode
                   , s.zscode
                   , s.kind
                   , s.kindDetail
                }).Distinct().ToList();

                var json = JsonConvert.SerializeObject(list);
                File.WriteAllText(path, json.ToString());
            }


            var item2 = new List<ChargerItem>();
            var item3 = new List<ChargerModel>();


            if (this.ExistFile(path))
            {
                var resp = JsonConvert.DeserializeObject<List<ChargerItem>>(File.ReadAllText(path));

                if(resp.Count() > 0)
                {
                    item2 = resp.Where(w => w.zscode == req.zscode && w.zcode == req.zcode).Distinct().ToList();
                }
            }
            
            var radius = Convert.ToDouble(_configuration.GetSection("CHARGER:SEARCH_RADIUS").Value);
            // 거리 계산
            Parallel.ForEach(item2, i => {

                if (i != null)
                {

                    i.distance = CommonUtils.DistanceTo(Convert.ToDouble(req.lng),
                                            Convert.ToDouble(req.lat),
                                            Convert.ToDouble(i.lng),
                                            Convert.ToDouble(i.lat));

                    var model = new ChargerModel();
                    model.statNm = i.statNm;
                    model.statId = i.statId;
                    model.location = i.location;
                    model.lat = i.lat;
                    model.lng = i.lng;
                    model.addr = i.addr;
                    model.kind = i.kind;
                    model.kindDetail = i.kindDetail;

                    model.distance = i.distance;

                    item3.Add(model);
                }

            });

            
            return item3.Where(w => w!= null && w.distance <= radius).ToList();
        }

        public List<ChargerItem> GetChargerAPIList(ChargerReqModel req)
        {
            var res = this.ChargerAPI(req, 1);
            var itemList = res.items.item;

            var cnt = res.totalCount / res.numOfRows;
            var mod = (res.totalCount % res.numOfRows > 0) ? cnt++ : cnt;

            Parallel.For(0, cnt, (i) =>
            {
                ++req.pageNo;
                var res2 = this.ChargerAPI(req, 1);

                itemList = itemList.Union(res2.items.item).ToList();
            });


            return itemList;

        }


        public List<ChargerStatusItem> GetChargerStatusAPIList(ChargerReqModel req)
        {
            var res = this.ChargerStatusAPI(req);
            var itemList = res.items.item;

            var cnt = res.totalCount / res.numOfRows;
            var mod = (res.totalCount % res.numOfRows > 0) ? cnt++ : cnt;

            Parallel.For(0, cnt, (i) =>
            {
                ++req.pageNo;
                var res2 = this.ChargerStatusAPI(req);

                itemList = itemList.Union(res2.items.item).ToList();
            });


            return itemList;

        }


        public string GetZcode(ChargerReqModel req)
        {
            var zcode = "";

            var zcodeList = _chargerConfig.Value.zcode;

            if (string.IsNullOrEmpty(req.depth1) == false)
            {
                var list = zcodeList.Where(w => w.name.Contains(req.depth1));

                if (list.Count() > 0)
                {
                    zcode = list.First().code;
                }
            }

            return zcode;
        }

        public string GetZScode(ChargerReqModel req)
        {
            var zscode = "";

            var zscodeList = _chargerConfig.Value.zscode;

            if (string.IsNullOrEmpty(req.depth2) == false)
            {
                var list = zscodeList.Where(w => w.code.Substring(0,2).Equals(req.zcode) &&  w.name == req.depth2);

                if (list.Count() > 0)
                {
                    zscode = list.First().code;
                }
            }

            return zscode;
        }

        public string GetCodeNm(CodeType codeType, string code)
        {
            var name = "";

            var codeList = new List<Value>();

            switch (codeType)
            {
                case CodeType.busid:
                    codeList = _chargerConfig.Value.busid;
                    break;
                case CodeType.stat:
                    codeList = _chargerConfig.Value.stat;
                    break;
                case CodeType.chgerType:
                    codeList = _chargerConfig.Value.chgerType;
                    break;
                case CodeType.zcode:
                    codeList = _chargerConfig.Value.zcode;
                    break;
                case CodeType.zscode:
                    codeList = _chargerConfig.Value.zscode;
                    break;
                case CodeType.kind:
                    codeList = _chargerConfig.Value.kind;
                    break;
                case CodeType.kindDetail:
                    codeList = _chargerConfig.Value.kindDetail;
                    break;
            }
            

            var list = codeList.Where(w => w.code.Equals(code));

            if (list.Count() > 0)
            {
                name = list.First().name;
            }

            return name;
        }


        private ChargerResModel ChargerAPI(ChargerReqModel req, int? realtime)
        {
            //var model = new ChargerResModel();

            var path = _hostingEnvironment.WebRootPath + "/" + _configuration.GetSection("CHARGER:CHARGER_LIST_JSON").Value;
            path = path.Replace("{}", req.pageNo.ToString());


            string url = req.searchurl; // URL
            url += "?ServiceKey=" + req.serviceKey; // Service Key
            url += "&pageNo=" + req.pageNo;
            url += "&numOfRows=" + req.numOfRows;
            url += "&zcode=" + req.zcode;

            if (realtime == 1)
            { 
                url += "&zscode=" + req.zscode;
                url += "&kind=" + req.kind;
                url += "&kindDetail=" + req.kindDetail;
            }

            url += "&dataType=JSON";


            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            string results = string.Empty;
            HttpWebResponse response;
            using (response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                results = reader.ReadToEnd();

                //using (FileStream fs = File.Create(path))
                //{
                //    Byte[] json = new UTF8Encoding(true).GetBytes(results);
                //    fs.Write(json, 0, json.Length);
                //}

                // 응답데이터를 json 파일로 생성
                return JsonConvert.DeserializeObject<ChargerResModel>(results);
            }
        }

        private ChargerStatusResModel ChargerStatusAPI(ChargerReqModel req)
        {
            //var model = new ChargerResModel();

            string url = req.searchurl; // URL
            url += "?ServiceKey=" + req.serviceKey; // Service Key
            url += "&pageNo=" + req.pageNo;
            url += "&numOfRows=" + req.numOfRows;
            url += "&period=5";
            url += "&zcode=" + req.zcode;
            url += "&zscode=" + req.zscode;
            url += "&kind=" + req.kind;
            url += "&kindDetail=" + req.kindDetail;
            url += "&dataType=JSON";


            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            string results = string.Empty;
            HttpWebResponse response;
            using (response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                results = reader.ReadToEnd();

                // 응답데이터를 json 파일로 생성
                return JsonConvert.DeserializeObject<ChargerStatusResModel>(results);
            }
        }


        private bool ExistFile(string path)
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


        private bool ExistDirectoryFile(string path)
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

    }    
}

