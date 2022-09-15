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
            
            //ViewBag.Name =  _userService.GetUser(this.User, "name");

            return View();
        }


        [HttpPost]
        public IActionResult Search(string search)
        {
            var response = new CampResModel();
            if (string.IsNullOrEmpty(search))
            {
                CampReqModel req = new CampReqModel
                {
                    serviceKey = _configuration.GetSection("OPENAPI:PUBLIC_API_KEY").Value,
                    searchurl = _configuration.GetSection("OPENAPI:PUBLIC_API_BASE_URL").Value,
                    pageNo = 1,
                    numOfRows = 1000,
                    MobileOS = "ETC",
                    MobileApp = "CampView"
                };


                response = this.CampList(req);
            }
            else
            {
                CampReqModel req = new CampReqModel { 
                    keyword = search
                };

                response = this.CampList(req);
            }

            return new JsonResult(response);
        }


        private CampResModel CampList(CampReqModel req)
        {
            var response = new CampResModel();

            
            if (string.IsNullOrEmpty(req.keyword) == false)
            {
                response = _campService.GetCampSearch(req.keyword);
            }
            else
            {
                if(string.IsNullOrEmpty(req.mapX))
                {
                    response = _campService.GetCampList(req);
                }
                else
                {
                    response = _campService.GetCampLocationList(req);
                }
            }

            response.response.body.items.item = response.response.body.items.item.Where(w => w.facltDivNm != "민간").ToList();


            // todo 검색 조건에 때래서 필터링

            return response;
        }



        [HttpPost]
        public IActionResult SearchLocation(CampReqModel req)
        {
            CampReqModel model = new CampReqModel
            {
                serviceKey = _configuration.GetSection("OPENAPI:PUBLIC_API_KEY").Value,
                searchurl = _configuration.GetSection("OPENAPI:PUBLIC_API_LOCATION_URL").Value,
                pageNo = 1,
                numOfRows = 300,
                MobileOS = "ETC",
                MobileApp = "CampView",
                mapX = req.mapX,
                mapY = req.mapY,
                radius = 20000
            };

            var response = this.CampList(model);

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


    }
}
