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
            var name = User.Claims.FirstOrDefault(x => x.Type == "sns").Value;
            var id = User.Claims.FirstOrDefault(x => x.Type == "id").Value;

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
            req.userid = base.GetUserId();

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
            var response = _userService.GetBlogList(query);

            return new JsonResult(response);
        }


        [HttpGet]
        public IActionResult SetCampRedis()
        {
            return new JsonResult("ok");
        }


        public IActionResult Redis(string action, string key, string value )
        {
            return View();
        }

        [HttpPost]
        public IActionResult Keyword(string query)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(base.GetUserId()) == false)
            {
                dic = _campService.GetkeywordList(base.GetUserId());
            }

            return new JsonResult(dic.ToList());
        }


        [HttpPost]
        public IActionResult KeywordDel(string query)
        {
            Dictionary<string, bool> dic = new Dictionary<string, bool>();
            if (string.IsNullOrEmpty(base.GetUserId()) == false)
            {
                bool isOk = _campService.KeywordDel(query, base.GetUserId());

                dic.Add("result", isOk);
            }

            return new JsonResult(dic.ToList());
        }


        [HttpPost]
        public IActionResult Comments(string contentid)
        {
            var list = new List<CampComment>();
            if (string.IsNullOrEmpty(base.GetUserId()) == false)
            {
                list = _campService.GetComments(base.GetUserId(), contentid);
            }

            return new JsonResult(list);
        }

        [HttpPost]
        public IActionResult AddComment(CampComment comm)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(base.GetUserId()) == false)
            {
                comm.user = base.GetUserId();
                comm.ymd = DateTime.Now.ToString("yyyy.MM.dd");
                bool isOk = false;
                var msg = "";
                if (_campService.ChkComment(comm))
                {
                    isOk = false;
                    msg = "already added";
                }
                else
                {
                    isOk = _campService.SetComment(comm);
                    if(isOk)
                    {
                        msg = "ok";
                    }
                }
                

                dic.Add("result", isOk.ToString());
                dic.Add("message", msg);
            }
            else
            {
                dic.Add("result", "False");
                dic.Add("message", "Require Login");
            }

            return new JsonResult(dic.ToList());
        }

        [HttpPost]
        public IActionResult DelComment(CampComment comm)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(base.GetUserId()) == false)
            {
                comm.user = base.GetUserId();
                
                bool isOk = false;
                var msg = "";
                if (_campService.ChkComment(comm))
                {
                    // contentid 의 데이터를 가져온다.
                    var data = _campService.GetComments(base.GetUserId(), comm.contentid).Where(w => w.me);

                    if (data != null && data.Count() > 0)
                    {
                        data.FirstOrDefault().user = base.GetUserId();

                        isOk = _campService.DelComment(data.FirstOrDefault());
                        msg = "ok";
                    }
                }
                else
                {
                    msg = "comment not find";
                }


                dic.Add("result", isOk.ToString());
                dic.Add("message", msg);
            }
            else
            {
                dic.Add("result", "False");
                dic.Add("message", "Require Login");
            }

            return new JsonResult(dic.ToList());

        }

    }
}
