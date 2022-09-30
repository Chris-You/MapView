using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CampView.Models;
using CampView.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace CampView.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;

        private IAccountService _accountService;
        private readonly IUserService _userService;

        public AccountController(ILogger<HomeController> logger, IConfiguration config,
                              IAccountService accountService,IUserService userService)
        {
            _logger = logger;
            _configuration = config;
            _accountService = accountService;
            _userService = userService;
        }

        public IActionResult Login()
        {
            return View();
        }

        
        public IActionResult NaverLogin()
        {
            var clientId = _configuration.GetSection("OPENAPI:NAVER_CLIENT_ID").Value;
            var redirectURI = string.Format("{0}://{1}{2}", Request.Scheme, Request.Host, "/Account/NaverLoginCallBack");
            var state = DateTime.Now.ToString("yyyyMMddHHmmssmi");
            var apiURL = "https://nid.naver.com/oauth2.0/authorize?response_type=code&client_id="
                + clientId + "&redirect_uri=" + redirectURI + "&state=" + state;

            return new RedirectResult(apiURL);
        }
                
        public async Task<IActionResult> NaverLoginCallBack(string code, string state)
        { 
            var id = string.Empty;
            var token = await _accountService.GetAccessToken(code, state);

            if (token != null)
            {
                var profile = _accountService.GetUserProfile(token.access_token);

                if ("00" == profile.resultcode && string.IsNullOrEmpty(profile.response.id) == false)
                {
                    CookieUserModel user = new CookieUserModel {
                        Sns = "naver",
                        Id = profile.response.id,
                        Name = profile.response.name,
                        Email = profile.response.email
                    };

                    // 로그인 성ㅅ공
                    await _userService.SignIn(this.HttpContext, user, false);

                    
                }
            }

            return LocalRedirect("~/Home/Main");
        }


        public async Task<IActionResult> GoogleLogin(string id, string name, string email)
        {

            CookieUserModel user = new CookieUserModel
            {
                Sns = "google",
                Id = id,
                Name = name,
                Email = email
            };

            // 로그인 성공
            await _userService.SignIn(this.HttpContext, user, false);

            return LocalRedirect("~/Home/Main");
        }


        public IActionResult KakaoLogin()
        {
            var restApiKey = _configuration.GetSection("OPENAPI:KAKAO_REST_KEY").Value;
            var redirectURI = string.Format("{0}://{1}{2}", Request.Scheme, Request.Host, "/Account/KakaoLoginCallBack");
            var state = DateTime.Now.ToString("yyyyMMddHHmmssmi");
            var apiURL = "https://kauth.kakao.com/oauth/authorize?client_id="+ restApiKey + "&redirect_uri="+ redirectURI + "&response_type=code&state=" + state;


            return new RedirectResult(apiURL);
        }


        public async Task<IActionResult> KakaoLoginCallBack(string code, string state)
        {
            var id = string.Empty;
            var redirectUrl = string.Format("{0}://{1}{2}", Request.Scheme, Request.Host, "/Account/KakaoLoginCallBack");

            var token = await _accountService.GetKakaoAccessToken(code, state, redirectUrl);

            if (token != null)
            {
                var profile = _accountService.GetKakaoUserProfile(token.access_token);

                if (string.IsNullOrEmpty(profile.id) == false)
                {
                    CookieUserModel user = new CookieUserModel
                    {
                        Sns = "kakao",
                        Id = profile.id,
                        Name = profile.kakao_account.name != null ? profile.kakao_account.name : profile.kakao_account.profile.nickname,
                        Email = profile.kakao_account.email_needs_agreement ? profile.kakao_account.email : ""
                    };

                    // 로그인 성ㅅ공
                    await _userService.SignIn(this.HttpContext, user, false);


                }
            }

            return LocalRedirect("~/Home/Main");
        }


        public async Task<IActionResult> LogoutAsync()
        {
            await _userService.SignOut(this.HttpContext);
            return RedirectPermanent("~/Home/Main");
        }

        [Authorize]
        public IActionResult Profile()
        {
            var dic = this.User.Claims.ToDictionary(x => x.Type, x => x.Value);

            return View(dic);
        }
    }
}
