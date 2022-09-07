using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CampingView.Models;
using CampingView.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace CampingView.Controllers
{
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;

        private INaverService _naverService;
        private readonly IUserService _userService;

        public AccountController(ILogger<HomeController> logger, IConfiguration config,
                              INaverService naverService,IUserService userService)
        {
            _logger = logger;
            _configuration = config;
            _naverService = naverService;
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
            var token = await _naverService.GetAccessToken(code, state);

            if (token != null)
            {
                var profile = _naverService.GetUserProfile(token.access_token);

                if ("00" == profile.resultcode && string.IsNullOrEmpty(profile.response.id) == false)
                {
                    CookieUserModel user = new CookieUserModel { 
                        Name = profile.response.name,
                        Email = profile.response.email
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
