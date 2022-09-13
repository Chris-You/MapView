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

namespace CampingView.Services
{
    public interface IAccountService
    {
        SearchResModel GetBlogList(string query);
        Task<NaverAccessToken> GetAccessToken(string code, string state);
        NaverUserProfile GetUserProfile(string token);


        Task<KakaoAccessToken> GetKakaoAccessToken(string code, string state, string redirectUrl);
        KakaoUserProfile GetKakaoUserProfile(string token);
    }

    public class AccountService : IAccountService
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;

        private string _clientId = string.Empty;
        private string _clientSecret = string.Empty;
        private string _searechBlogUrl = string.Empty;


        public AccountService(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _clientId = _configuration.GetSection("OPENAPI:NAVER_CLIENT_ID").Value;
            _clientSecret = _configuration.GetSection("OPENAPI:NAVER_CLIENT_SECRET").Value;
            _searechBlogUrl = _configuration.GetSection("OPENAPI:NAVER_SEARCH_BLOG_URL").Value;
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
                Console.WriteLine("Error 발생=" + status);
                //Error Logging
            }


            return model;
        }

        public async Task<NaverAccessToken> GetAccessToken(string code, string state)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-Naver-Client-Id", _clientId);
                client.DefaultRequestHeaders.Add("X-Naver-Client-Secret", _clientSecret);

                //string redirectURI = "YOUR-CALLBACK-URL";
                string apiURL = "https://nid.naver.com/oauth2.0/token?grant_type=authorization_code&";
                apiURL += "client_id=" + _clientId;
                apiURL += "&client_secret=" + _clientSecret;
                //apiURL += "&redirect_uri=" + redirectURI;
                apiURL += "&code=" + code;
                apiURL += "&state=" + state;

                var res = await client.GetAsync(apiURL);
                var responseString = await res.Content.ReadAsStringAsync();
                //Console.WriteLine("res.StatusCode = " + res.StatusCode);
                //return "res.StatusCode=" + res.StatusCode + "::: responseString" + responseString.ToString();

                var response = JsonConvert.DeserializeObject<NaverAccessToken>(responseString);

                return response;
            }
        }

        public NaverUserProfile GetUserProfile(string token)
        {
            string header = "Bearer " + token; // Bearer 다음에 공백 추가
            string apiURL = "https://openapi.naver.com/v1/nid/me";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiURL);
            request.Headers.Add("X-Naver-Client-Id", _clientId);
            request.Headers.Add("X-Naver-Client-Secret", _clientSecret);
            request.Headers.Add("Authorization", header);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string status = response.StatusCode.ToString();
            if (status == "OK")
            {
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                string text = reader.ReadToEnd();
                //Console.WriteLine(text);

                var profile = JsonConvert.DeserializeObject<NaverUserProfile>(text);

                return profile;
            }
            else
            {
                Console.WriteLine("Error 발생=" + status);

                return null;
            }
        }


        public async Task<KakaoAccessToken> GetKakaoAccessToken(string code, string state, string redirectUrl)
        {
            using (var client = new HttpClient())
            {
                
                //string redirectURI = "YOUR-CALLBACK-URL";
                string apiURL = "https://kauth.kakao.com/oauth/token";

                var parameters = new Dictionary<string, string>();
                parameters.Add("grant_type", "authorization_code");
                parameters.Add("client_id", _configuration.GetSection("OPENAPI:KAKAO_REST_KEY").Value);
                parameters.Add("redirect_uri", redirectUrl);
                parameters.Add("code", code);
                var encodedContent = new FormUrlEncodedContent(parameters);

                var res = await client.PostAsync(apiURL, encodedContent);
                var responseString = await res.Content.ReadAsStringAsync();
                //Console.WriteLine("res.StatusCode = " + res.StatusCode);
                //return "res.StatusCode=" + res.StatusCode + "::: responseString" + responseString.ToString();

                var response = JsonConvert.DeserializeObject<KakaoAccessToken>(responseString);

                return response;
            }
        }

        public KakaoUserProfile GetKakaoUserProfile(string token)
        {
            string header = "Bearer " + token; // Bearer 다음에 공백 추가
            string apiURL = "https://kapi.kakao.com/v2/user/me";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiURL);
            request.Headers.Add("Authorization", header);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            string status = response.StatusCode.ToString();
            if (status == "OK")
            {
                Stream stream = response.GetResponseStream();
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                string text = reader.ReadToEnd();
                //Console.WriteLine(text);

                var profile = JsonConvert.DeserializeObject<KakaoUserProfile>(text);

                return profile;
            }
            else
            {
                Console.WriteLine("Error 발생=" + status);

                return null;
            }
        }

    }
}
