using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using MapView.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using MapView.Util;
using StackExchange.Redis;

namespace MapView.Services
{

    public interface IFestivalService
    {
        FestivalResModel GetFestivalList(FestivalReqModel req);

        /*
        Dictionary<string, string> GetkeywordList(string userid);
        bool KeywordDel(string word, string userid);

        List<CampComment> GetComments(string userid, string contentid);
        string GetComment(string userid, string contentid);
        bool SetComment(CampComment comment);
        bool DelComment(CampComment comment);
        bool ChkComment(CampComment comment);

        bool SetLike(string userid, string contentid);
        bool DelLike(string userid, string contentid);
        bool ChkLike(string userid, string contentid);
        
        SearchResModel GetBlogList(string query);

        */
    }

    public class FestivalService : IFestivalService
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpClientFactory _clientFactory;

        private string _clientId = string.Empty;
        private string _clientSecret = string.Empty;
        private string _searechBlogUrl = string.Empty;

        private readonly RedisService _redis;

        public FestivalService(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _clientFactory = clientFactory;

            _clientId = _configuration.GetSection("OPENAPI:NAVER_CLIENT_ID").Value;
            _clientSecret = _configuration.GetSection("OPENAPI:NAVER_CLIENT_SECRET").Value;

            _redis = new RedisService(
                            _configuration.GetSection("REDIS:SERVER").Value.ToString(),
                            _configuration.GetSection("REDIS:PORT").Value.ToString(),
                            _configuration.GetSection("REDIS:PASSWORD").Value.ToString());
        }



        public Dictionary<string,string> GetkeywordList(string userid)
        {

            var redisKey = _configuration.GetSection("REDIS:SEARCH_KEY").Value.ToString() + userid;

            Dictionary<string, string> dic = new Dictionary<string, string>();
            
            foreach (var k in _redis.redisDatabase.SortedSetRangeByRankWithScores(redisKey, 0, -1, order: Order.Descending))
            {
                var key = k.ToString().Split(":")[0];
                var date = k.ToString().Split(":")[1];
                
                dic.Add(key, string.Format("{0}.{1}", date.Substring(5, 2), date.Substring(7, 2)));
            }

            return dic;
        }

        public bool KeywordDel(string word, string userid)
        {
            var redisKey = _configuration.GetSection("REDIS:SEARCH_KEY").Value.ToString() + userid;

            return _redis.redisDatabase.SortedSetRemove(redisKey, word);
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
                for (var i=1; i<= cnt; i++)
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

                if (resp.response.body.items != null && resp.response.body.items.Count() > 0)
                {
                    arr.Add(resp.response.body.items);
                }
            });
            
            var item = new List<FestivalItem>();
            var dt = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));
            var dt2 = Convert.ToInt32(DateTime.Now.AddMonths(5).ToString("yyyyMMdd"));



            foreach (var li in arr)
            {
                item = item.Union(li.Where(w => Convert.ToInt32(w.fstvlStartDate.Replace("-", "")) >= dt && 
                                                Convert.ToInt32(w.fstvlStartDate.Replace("-", "")) <= dt2)).ToList();
            }

            Random rand = new Random();
            var shuffled = item.OrderBy(o => rand.Next()).ToList();

            model.response.header.resultCode = "00";
            model.response.header.resultMsg = "OK";
            model.response.body.items = shuffled;
            model.response.body.numOfRows = item.Count();
            model.response.body.totalCount = item.Count();
            model.response.body.pageNo = 1;

            return model;
        }


        
        public List<CampComment> GetComments(string userid, string contentid)
        {
            var redisKey = _configuration.GetSection("REDIS:COMMENT_KEY").Value.ToString() + contentid ;

            List<CampComment> list = new List<CampComment>();
            //_redis.redisDatabase.SortedSetRangeByRankWithScores(redisKey, 0, -1, order: Order.Descending);
            foreach (var i in _redis.redisDatabase.SetMembers(redisKey))
            {
                var splt =  i.ToString().Split(":::");
                
                list.Add(new CampComment
                        {
                            user = splt[0].Substring(0,5) + "***",
                            ymd = splt[1],
                            comment = splt[2],
                            contentid = contentid,
                            me = (splt[0] == userid) ? true :false
                        }
                );
            }

            return list.OrderByDescending(o => o.me).ToList();
        }

        public string GetComment(string userid, string contentid)
        {
            var redisKey = _configuration.GetSection("REDIS:COMMENT_KEY").Value.ToString() + contentid;
            var value = string.Empty;

            List<CampComment> list = new List<CampComment>();
            //_redis.redisDatabase.SortedSetRangeByRankWithScores(redisKey, 0, -1, order: Order.Descending);
            foreach (var i in _redis.redisDatabase.SetMembers(redisKey))
            {
                var splt = i.ToString().Split(":::");

                if(splt[0] == userid)
                {
                    value = i.ToString();
                    break;
                }
            }

            return value;
            
        }


        public bool ChkComment(CampComment comment)
        {
            var redisKey = _configuration.GetSection("REDIS:COMMENT_KEY").Value.ToString() + comment.contentid;
            bool isOk = false;
            foreach (var i in _redis.redisDatabase.SetMembers(redisKey))
            {
                var splt = i.ToString().Split(":::");

                if(splt[0] == comment.user)
                {
                    isOk = true;
                    break;
                }
            }

            return isOk;
        }
        


        public bool SetComment(CampComment comment)
        {
            var redisKey = _configuration.GetSection("REDIS:COMMENT_KEY").Value.ToString() + comment.contentid;
            var value = comment.user +":::"+ DateTime.Now.ToString("yyyy.MM.dd") + ":::" + comment.comment;

            return _redis.redisDatabase.SetAdd(redisKey, value);
        }

        public bool DelComment(CampComment comment)
        {
            var redisKey = _configuration.GetSection("REDIS:COMMENT_KEY").Value.ToString() + comment.contentid;
            var value = comment.user + ":::" + comment.ymd + ":::" + comment.comment;

            return _redis.redisDatabase.SetRemove(redisKey, value);
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


        

        private FestivalResModel MakeFileByAPI(FestivalReqModel req)
        {
            var path = _hostingEnvironment.WebRootPath + "/" + _configuration.GetSection("FESTIVAL:LIST_JSON").Value;
            path = path.Replace("{}", req.pageNo.ToString());


            string url = req.searchurl; // URL
            url += "?ServiceKey=" + req.serviceKey; // Service Key
            url += "&pageNo=" + req.pageNo;
            url += "&numOfRows=" + req.numOfRows;
            url += "&type=json";


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
        
    }
}
