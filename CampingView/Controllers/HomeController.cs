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
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<HomeController> _logger;

        private ICampService _campService;

        

        public HomeController(ILogger<HomeController> logger, ICampService campService, IConfiguration config)
        {
            _logger = logger;
            _campService = campService;
            _configuration = config;
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


        [HttpPost]
        public IActionResult SearchBase()
        {
            CampReqModel req = new CampReqModel
            {
                serviceKey = _configuration.GetSection("AppSettings:API_KEY").Value,
                searchurl = _configuration.GetSection("AppSettings:API_BASE_URL").Value,
                pageNo = 1,
                numOfRows = 1000,
                MobileOS = "ETC",
                MobileApp = "CampView"
            };

            var response = _campService.GetCampList(req);

            return new JsonResult(response);
        }


        [HttpPost]
        public IActionResult SearchLocation(CampReqModel req)
        {
            CampReqModel model = new CampReqModel
            {
                serviceKey = _configuration.GetSection("AppSettings:API_KEY").Value,
                searchurl = _configuration.GetSection("AppSettings:API_LOCATION_URL").Value,
                pageNo = 1,
                numOfRows = 300,
                MobileOS = "ETC",
                MobileApp = "CampView",
                mapX = req.mapX,
                mapY = req.mapY,
                radius = 2000
            };

            var response = _campService.GetCampList(model);

            return new JsonResult(response);
        }


        [HttpPost]
        public IActionResult SearchImage(CampReqModel req)
        {
            CampReqModel model = new CampReqModel
            {
                serviceKey = _configuration.GetSection("AppSettings:API_KEY").Value,
                searchurl = _configuration.GetSection("AppSettings:API_IMAGE_URL").Value,
                MobileOS = "ETC",
                MobileApp = "CampView",
                contentId = req.contentId
            };

            var response = _campService.GetCampImgList(model);

            return new JsonResult(response);
        }


        public IActionResult Main()
        {
            return View();
        }
    }
}
