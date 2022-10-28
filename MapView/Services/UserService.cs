
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using MapView.Common.Models;
using MapView.Common.Models.Charger;
using MapView.Common.Models.Festival;
using Microsoft.Extensions.Configuration;
using System.Linq;
using StackExchange.Redis;
using System.Net;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Hosting;
using System.Text;
using MapView.Common.Database;
using System;

namespace MapView.Services
{
    public interface IUserService
    {
        Task SignIn(HttpContext httpContext, CookieUserModel user, bool isPersistent = false);
        Task SignOut(HttpContext httpContext);

        //string GetUser(ClaimsPrincipal principal, string claim);
        //Dictionary<string, string> GetUser(ClaimsPrincipal principal);

        Faq RegFaq(Faq faq);


        List<Favor> FavorList(string userid, ServiceGubun service);
        bool InsFavor(string userid, ServiceGubun service, string statId, string zscode);
        bool DelFavor(string userid, ServiceGubun service, string statId);
        bool ChkFavor(string userid, ServiceGubun service, string statId);

    }

    public class UserService : BaseService, IUserService
    {
        private readonly IConfiguration _configuration;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly Redis _redis;
        private readonly Mongo _mongoDB;

        public UserService(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;

            _redis = new Redis(
                           _configuration.GetSection("REDIS:SERVER").Value.ToString(),
                           _configuration.GetSection("REDIS:PORT").Value.ToString(),
                           _configuration.GetSection("REDIS:PASSWORD").Value.ToString());

            var host = _configuration.GetSection("MONGODB:SERVER").Value.ToString() + ":" + _configuration.GetSection("MONGODB:PORT").Value.ToString();

            _mongoDB = new Mongo(_configuration.GetSection("MONGODB:USER").Value.ToString(),
                            host,
                            _configuration.GetSection("MONGODB:DB_NAME").Value.ToString());
        }

        /*
        public Dictionary<string, string> GetUser(ClaimsPrincipal principal)
        {
            return principal.Claims.ToDictionary(x => x.Type, x => x.Value);
        }


        public string GetUser(ClaimsPrincipal principal, string claim)
        {
            var dic = principal.Claims.ToDictionary(x => x.Type, x => x.Value);

            if (dic.ContainsKey(claim))
                return dic[claim].ToString();
            else
                return string.Empty;
        }
        */

        public async Task SignIn(HttpContext httpContext, CookieUserModel user, bool isPersistent = false)
        {
            string authenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            
              
            // Generate Claims from DbEntity
            var claims = GetUserClaims(user);

            // Add Additional Claims from the Context
            // which might be useful
            // claims.Add(httpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name));

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, authenticationScheme);
            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authProperties = new AuthenticationProperties
            {
                // AllowRefresh = <bool>,
                // Refreshing the authentication session should be allowed.
                // ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10),
                // The time at which the authentication ticket expires. A 
                // value set here overrides the ExpireTimeSpan option of 
                // CookieAuthenticationOptions set with AddCookie.
                // IsPersistent = true,
                // Whether the authentication session is persisted across 
                // multiple requests. Required when setting the 
                // ExpireTimeSpan option of CookieAuthenticationOptions 
                // set with AddCookie. Also required when setting 
                // ExpiresUtc.
                // IssuedUtc = <DateTimeOffset>,
                // The time at which the authentication ticket was issued.
                // RedirectUri = "~/Account/Index"
                // The full path or absolute URI to be used as an http 
                // redirect response value.
            };

            await httpContext.SignInAsync(authenticationScheme, claimsPrincipal, authProperties);

            this.RedisSession(user);
        }


        private void RedisSession(CookieUserModel user)
        {
            var userKey = _configuration.GetSection("REDIS:USER_KEY").Value.ToString() + user.Sns + ":" + user.Id;

            if (!_redis.redisDatabase.KeyExists(userKey))
            {
                HashEntry[] hash =
                {
                    new HashEntry("sns", user.Sns),
                    new HashEntry("id", user.Id),
                    new HashEntry("name", user.Name),
                    //new HashEntry("email", user.Email)
                };

                _redis.redisDatabase.HashSet(userKey, hash);

                //var name = redis.HashGet(userKey, "name");
                //var email = redis.HashGet(userKey, "email");
            }
        }

        public async Task SignOut(HttpContext httpContext)
        {
            await httpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }

        private List<Claim> GetUserClaims(CookieUserModel user)
        {
            List<Claim> claims = new List<Claim>();
            //claims.Add(new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()));
            claims.Add(new Claim("name", user.Name));
            //claims.Add(new Claim("email", user.Email));
            claims.Add(new Claim("sns", user.Sns));
            claims.Add(new Claim("id", user.Id));
            return claims;
        }



        public Faq RegFaq(Faq faq)
        {
            var docName = _configuration.GetSection("MONGODB:USER_FAQ").Value;

            _mongoDB.InsData<Faq>(faq, docName);

            var newFaq = _mongoDB.DataListByUser<Faq>(docName, faq.user).OrderByDescending(o => o.regDate).First() ;

            return newFaq;
        }



        public List<Favor> FavorList(string userid, ServiceGubun gubun)
        {
            var docName = _configuration.GetSection("MONGODB:MAPVIEW_FAVOR").Value;

            var list = _mongoDB.DataListByUser<Favor>(docName, userid).Where( w=> w.service == gubun).ToList();

            if (list != null)
            {
                foreach (var item in list)
                {
                    if (gubun == ServiceGubun.charger)
                    {
                        var path = _hostingEnvironment.WebRootPath + "/" + _configuration.GetSection("CHARGER:CHARGER_LIST_JSON").Value;
                        path = path.Replace("{}", item.zscode.Substring(0, 2));

                        if (base.ExistFile(path))
                        {
                            var resp = JsonConvert.DeserializeObject<List<ChargerItem>>(File.ReadAllText(path));

                            if (resp.Count() > 0)
                            {
                                var tmp = resp.Where(w => w.statId == item.contentId);
                                if (tmp != null && tmp.Count() > 0)
                                {
                                    item.contentNm = tmp.First().statNm;
                                    item.addr = tmp.First().addr;
                                }
                            }
                        }
                    }
                }
            }


            return list;
        }

        /*
        public List<ChargerFavor> FavorList(string statId)
        {
            var docName = _configuration.GetSection("MONGODB:CHARGER_FAVOR").Value;

            return _mongoDB.DataList<ChargerFavor>(docName, statId);
        }
        */

        public bool InsFavor(string user, ServiceGubun service, string statId, string zscode)
        {
            var isOk = false;
            try
            {
                var docName = _configuration.GetSection("MONGODB:MAPVIEW_FAVOR").Value;
                var doc = _mongoDB.GetData<Favor>(user, service, statId, docName);

                if (doc == null)
                {
                    var favor = new Favor();
                    favor.contentId = statId;
                    favor.user = user;
                    favor.service = service;
                    favor.zscode = zscode;
                    favor.date = DateTime.Now;

                    _mongoDB.InsData<Favor>(favor, docName);

                    isOk = true;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

            return isOk;

        }

        public bool DelFavor(string user, ServiceGubun service, string contentId)
        {
            var docName = _configuration.GetSection("MONGODB:MAPVIEW_FAVOR").Value;

            return _mongoDB.DelData<Favor>(user, service, contentId, docName);
        }


        public bool ChkFavor(string userid, ServiceGubun service, string contentId)
        {
            var docName = _configuration.GetSection("MONGODB:MAPVIEW_FAVOR").Value;
            bool isOk = false;

            var data = _mongoDB.GetData<Favor>(userid, service, contentId, docName);
            if (data != null)
            {
                isOk = true;
            }


            return isOk;
        }

    }
}
