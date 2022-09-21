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

namespace CampingView.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;

        private ICampService _campService;
        private IAccountService _accountService;
        private IUserService _userService;

        public HomeController(ILogger<HomeController> logger, IConfiguration config, 
                              ICampService campService, IAccountService accountService,
                              IUserService userService)
        {
            _logger = logger;
            _configuration = config;
            _campService = campService;
            _accountService = accountService;
            _userService = userService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult Main()
        {
            return View();
        }


        [HttpPost]
        public IActionResult Search(CampReqModel req)
        {
            req.serviceKey = _configuration.GetSection("OPENAPI:PUBLIC_API_KEY").Value;
            req.searchurl = _configuration.GetSection("OPENAPI:PUBLIC_API_BASE_URL").Value;
            req.pageNo = 1;
            req.radius = 20000;
            req.numOfRows = 1000;
            req.MobileOS = "ETC";
            req.MobileApp = "CampView";

            var response = _campService.GetCampList(req);


            //todo  filter
            response = _campService.CampFilter(req, response);




            return new JsonResult(response);
        }


        [HttpPost]
        public IActionResult SearchImage(CampReqModel req)
        {
            CampReqModel model = new CampReqModel
            {
                serviceKey = _configuration.GetSection("OPENAPI:PUBLIC_API_KEY").Value,
                searchurl = _configuration.GetSection("OPENAPI:PUBLIC_API_IMAGE_URL").Value,
                MobileOS = "ETC",
                MobileApp = "CampView",
                contentId = req.contentId
            };

            var response = _campService.GetCampImgList(model);

            return new JsonResult(response);
        }


        [HttpPost]
        public IActionResult Blogs(string query)
        {
            var response = _accountService.GetBlogList(query);

            return new JsonResult(response);
        }


        [HttpGet]
        public IActionResult SetCampRedis()
        {
            _campService.SetRedis();

            return new JsonResult("ok");
        }


        public IActionResult Redis(string action, string key, string value )
        {
            return View();
        }

    }
}
