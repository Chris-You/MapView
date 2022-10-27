using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using MapView.Common.Models;
using MapView.Common.Models.Festival;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;
using System.Text;

using MapView.Common.Database;
using System.Xml;
using MapView.Common.Models.CustomSettings;
using Microsoft.Extensions.Options;



namespace MapView.Services
{

    public interface IFestivalService
    {
        //FestivalResModel GetFestivalList(FestivalReqModel req);


        FestivalResModel GetFestivalList(FestivalReqModel req);
        //FestivalDetailResModel GetFestivalDetail(FestivalDetailReqModel req);
        T GetFestivalDetail<T>(FestivalDetailReqModel req);


        T GetAreaCode<T>(FestivalReqModel req);

        string GetCustomCode(string codetype, string code);
        //string GetAreaCode(string code);

        SearchResModel GetBlogList(string query);
    }

    public class FestivalService : IFestivalService
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpClientFactory _clientFactory;

        private string _clientId = string.Empty;
        private string _clientSecret = string.Empty;
        private string _searechBlogUrl = string.Empty;
        private readonly IOptions<FestivalCode> _festivalConfig;

        private readonly Redis _redis;

        public FestivalService(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, IHttpClientFactory clientFactory,
                               IOptions<FestivalCode> festivalConfig)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _clientFactory = clientFactory;
            _festivalConfig = festivalConfig;

            _clientId = _configuration.GetSection("OPENAPI:NAVER_CLIENT_ID").Value;
            _clientSecret = _configuration.GetSection("OPENAPI:NAVER_CLIENT_SECRET").Value;
            _searechBlogUrl = _configuration.GetSection("OPENAPI:NAVER_SEARCH_BLOG_URL").Value;

            _redis = new Redis(
                            _configuration.GetSection("REDIS:SERVER").Value.ToString(),
                            _configuration.GetSection("REDIS:PORT").Value.ToString(),
                            _configuration.GetSection("REDIS:PASSWORD").Value.ToString());
        }


        public string GetCustomCode(string codeType, string code)
        {
            var zcode = "";
            var zcodeList = new List<itemValue>();
            if (codeType == "area")
            {
                zcodeList = _festivalConfig.Value.areaCode;
            }
            else if (codeType == "category")
            {
                zcodeList = _festivalConfig.Value.categoryCode;
            }



            var list = zcodeList.Where(w => w.code == code);

            if (list.Count() > 0)
            {
                zcode = list.First().name;
            }

            return zcode;
        }


       
        public bool ChkLike(string userid, string contentid)
        {
            var redisKey = _configuration.GetSection("REDIS:LIKE_KEY").Value.ToString() + userid;
            bool isOk = false;
            foreach (var i in _redis.redisDatabase.SetMembers(redisKey))
            {
                if (contentid == i.ToString())
                {
                    isOk = true;
                    break;
                }
            }

            return isOk;
        }


        public bool SetLike(string userid, string contentid)
        {
            var redisKey = _configuration.GetSection("REDIS:LIKE_KEY").Value.ToString() + userid;
            var value = contentid;

            return _redis.redisDatabase.SetAdd(redisKey, value);
        }

        public bool DelLike(string userid, string contentid)
        {
            var redisKey = _configuration.GetSection("REDIS:LIKE_KEY").Value.ToString() + userid;
            var value = contentid;

            return _redis.redisDatabase.SetRemove(redisKey, value);
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







        public FestivalResModel GetFestivalList(FestivalReqModel req)
        {
            var model = new FestivalResModel();
            List<List<FestivalItem>> arr = new List<List<FestivalItem>>();


            var path = _hostingEnvironment.WebRootPath + "/" + _configuration.GetSection("FESTIVAL:LIST_JSON").Value;
            path = path.Replace("{}", req.pageNo.ToString());

            if (this.ExistFile(path) == false)
            {
                var res = this.MakeFileByAPI(req);

                var cnt = res.response.body.totalCount / res.response.body.numOfRows;
                var mod = (res.response.body.totalCount % res.response.body.numOfRows > 0) ? cnt++ : cnt;
                for (var i = 1; i <= cnt; i++)
                {
                    req.pageNo++;
                    var res2 = this.MakeFileByAPI(req);
                }
            }


            var dir = _hostingEnvironment.WebRootPath + "/" + _configuration.GetSection("FESTIVAL:LIST_DIR").Value;

            var di = new DirectoryInfo(dir);

            Parallel.ForEach(di.GetFiles(), file => {

                var str = File.ReadAllText(file.FullName);

                var resp = JsonConvert.DeserializeObject<FestivalResModel>(str);

                if (resp.response.body.items != null && resp.response.body.items.item.Count() > 0)
                {
                    if (string.IsNullOrEmpty(req.keyword))
                    {
                        arr.Add(resp.response.body.items.item);
                    }
                    else
                    {
                        arr.Add(resp.response.body.items.item.Where(w => w.title.IndexOf(req.keyword) >= 0).ToList());
                    }
                }
            });

            var item = new List<FestivalItem>();
            var dt = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
            var dt2 = Convert.ToInt32(DateTime.Now.AddMonths(5).ToString("yyyyMMdd"));


            // startDb >= today  || endDt <= today


            foreach (var li in arr)
            {
                item = item.Union(li.Where(w => Convert.ToInt32(w.eventenddate.Replace("-", "")) >= dt)).ToList();
            }

            Random rand = new Random();
            var shuffled = item.OrderBy(o => rand.Next()).ToList();

            model.response.header.resultCode = "00";
            model.response.header.resultMsg = "OK";
            model.response.body.items.item = shuffled;
            model.response.body.numOfRows = item.Count();
            model.response.body.totalCount = item.Count();
            model.response.body.pageNo = 1;

            return model;
        }


        private FestivalResModel MakeFileByAPI(FestivalReqModel req)
        {
            var path = _hostingEnvironment.WebRootPath + "/" + _configuration.GetSection("FESTIVAL:LIST_JSON").Value;
            path = path.Replace("{}", req.pageNo.ToString());


            string url = req.searchurl; // URL
            url += "?ServiceKey=" + req.serviceKey; // Service Key
            url += "&pageNo=" + req.pageNo;
            url += "&numOfRows=" + req.numOfRows;
            url += "&MobileOS=" + req.MobileOS;
            url += "&MobileApp=" + req.MobileApp;
            url += "&listYN=" + req.listYN;
            url += "&eventStartDate=" + req.eventStartDate;
            url += "&_type="+ req._type;


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

                return JsonConvert.DeserializeObject<FestivalResModel>(results);
            }
        }




        public T GetAreaCode<T>(FestivalReqModel req)
        {
            string url = req.searchurl; // URL
            url += "?ServiceKey=" + req.serviceKey; // Service Key
            url += "&pageNo=" + req.pageNo;
            url += "&numOfRows=" + req.numOfRows;
            url += "&MobileOS=" + req.MobileOS;
            url += "&MobileApp=" + req.MobileApp;

            if (req.searchurl.Split("/").Last() != "areaCode")
            {
                url += "&listYN=" + req.listYN;
                url += "&eventStartDate=" + req.eventStartDate;
            }

            //url += "&_type=" + req._type;


            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            string results = string.Empty;
            HttpWebResponse response;
            using (response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                results = reader.ReadToEnd();

                // 응답데이터를 json 파일로 생성

                // xml 파싱
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(results);

                string json = JsonConvert.SerializeXmlNode(doc);

                return JsonConvert.DeserializeObject<T>(json);
            }
        }

        public T GetFestivalDetail<T>(FestivalDetailReqModel req)
        {

            string url = req.searchurl; // URL
            url += "?ServiceKey=" + req.serviceKey; // Service Key
            url += "&MobileOS=" + req.MobileOS;
            url += "&MobileApp=" + req.MobileApp;
            url += "&contentId=" + req.contentId;
            url += "&_type=" + req._type;

            if (req.searchurl.Split("/").Last() == "detailCommon")
            {
                url += "&defaultYN=Y";
                url += "&firstImageYN=Y";
                url += "&areacodeYN=Y";
                url += "&catcodeYN=Y";
                url += "&addrinfoYN=Y";
                url += "&mapinfoYN=Y";
                url += "&overviewYN=Y";
            }
            else if (req.searchurl.Split("/").Last() == "detailImage")
            {
                url += "&imageYN=Y";
                url += "&subImageYN=Y";
            }
            else if (req.searchurl.Split("/").Last() == "detailInfo")
            {
                url += "&contentTypeId=" + req.contentTypeId;
            }


            var request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";

            string results = string.Empty;
            HttpWebResponse response;
            using (response = request.GetResponse() as HttpWebResponse)
            {
                StreamReader reader = new StreamReader(response.GetResponseStream());
                results = reader.ReadToEnd();


                return JsonConvert.DeserializeObject<T>(results);
            }
        }


        public SearchResModel GetBlogList(string query)
        {
            var model = new SearchResModel();


            //string query = "네이버 Open API"; // 검색할 문자열
            string url = _searechBlogUrl + query; // 결과가 JSON 포맷
            // string url = "https://openapi.naver.com/v1/search/blog.xml?query=" + query;  // 결과가 XML 포맷
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Headers.Add("X-Naver-Client-Id", _clientId); // 개발자센터에서 발급받은 Client ID
            request.Headers.Add("X-Naver-Client-Secret", _clientSecret); // 개발자센터에서 발급받은 Client Secret
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string status = response.StatusCode.ToString();
            if (status == "OK")
            {
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                string text = reader.ReadToEnd();
                //Console.WriteLine(text);

                model = JsonConvert.DeserializeObject<SearchResModel>(text);

                model.items = model.items.OrderByDescending(o => o.postdate).ToList();
            }
            else
            {
                //Console.WriteLine("Error 발생=" + status);
                //Error Logging
            }


            return model;
        }

    }


}
