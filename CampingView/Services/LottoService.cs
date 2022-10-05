using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.IO;
using CampView.Models;
using Newtonsoft.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json.Linq;
using System.Text;
using StackExchange.Redis;

namespace CampView.Services
{

    public interface ILottoService
    {
        bool SetLottoData(int start, int end);
        LottoResModel GetPrize(string date);
        
    }


    public class LottoService : ILottoService
    {

        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IHttpClientFactory _clientFactory;

        private string _clientId = string.Empty;
        private string _clientSecret = string.Empty;
        private readonly RedisService _redis;


        public LottoService(IConfiguration configuration, IWebHostEnvironment hostingEnvironment, IHttpClientFactory clientFactory)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
            _clientFactory = clientFactory;

            //_clientId = _configuration.GetSection("OPENAPI:NAVER_CLIENT_ID").Value;
            //_clientSecret = _configuration.GetSection("OPENAPI:NAVER_CLIENT_SECRET").Value;

            _redis = new RedisService(
                            _configuration.GetSection("REDIS:SERVER").Value.ToString(),
                            _configuration.GetSection("REDIS:PORT").Value.ToString(),
                            _configuration.GetSection("REDIS:PASSWORD").Value.ToString(),
                            _configuration.GetSection("REDIS:LOTTO_DB_IDX").Value.ToString());

        }

        
        public LottoResModel GetPrize(string date)
        {
            var lotto = new LottoResModel();
            var key = _configuration.GetSection("REDIS:LOTTO_PRIZE").Value + date;

            var response = _redis.redisDatabase.StringGet(key);

            if(string.IsNullOrEmpty(response)== false)
            {
                lotto = JsonConvert.DeserializeObject<LottoResModel>(response);
            }
            

            return lotto;
        }

        public bool SetLottoData(int start, int end)
        {
            bool isOk = false;
            if (start > 0 && end > 0 && end > start)
            {
                for (var i = start; i < end; i++)
                {
                    string apiURL = _configuration.GetSection("LOTTO:API_URL").Value + i;
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(apiURL);
                    HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                    string status = response.StatusCode.ToString();
                    if (status == "OK")
                    {
                        Stream stream = response.GetResponseStream();
                        StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                        string text = reader.ReadToEnd();
                        //Console.WriteLine(text);
                        var lotto = JsonConvert.DeserializeObject<LottoResModel>(text);
                        if (lotto.returnValue == "success")
                        {
                            var key = _configuration.GetSection("REDIS:LOTTO_PRIZE").Value + lotto.drwNoDate;

                            isOk = _redis.redisDatabase.StringSet(key, text);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Error 발생=" + status);
                    }


                }
            }

            return isOk;
        }
    }
}
