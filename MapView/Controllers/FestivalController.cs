using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
//using MapView.Common.Models;
using MapView.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using MapView.Common.Models.Festival;

namespace MapView.Controllers
{
    public class FestivalController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<FestivalController> _logger;

        private IFestivalService _fstvlService;
        private IAccountService _accountService;
        private IUserService _userService;

        public FestivalController(ILogger<FestivalController> logger, IConfiguration config, 
                              IFestivalService fstvlService, IAccountService accountService,
                              IUserService userService)
        {
            _logger = logger;
            _configuration = config;
            _fstvlService = fstvlService;
            _accountService = accountService;
            _userService = userService;
        }

        public IActionResult Index()
        {
            
            return View();
        }


       
        [HttpPost]
        public IActionResult Search(FestivalReqModel req)
        {
            req.serviceKey = _configuration.GetSection("OPENAPI:PUBLIC_API_KEY").Value;
            req.searchurl = _configuration.GetSection("OPENAPI:FESTIVAL_API_SEARCH_URL").Value;
            req.pageNo = 1;
            req.numOfRows = 1000;
            req.MobileOS = "ETC";
            req.MobileApp = "MapView";
            req._type = "json";
            req.listYN = "Y";
            req.arrange = "A";
            req.eventStartDate = DateTime.Now.ToString("yyyyMMdd");
            req.userid = base.GetUserId();

            var response = _fstvlService.GetFestivalList(req);


            var json = new
            {
                response.response.body.numOfRows,
                response.response.body.pageNo,
                response.response.body.totalCount,
                items = (
                    from s in response.response.body.items.item
                    select new
                    {
                        s.addr1
                        , s.addr2
                        , s.booktour
                        , cat1 = _fstvlService.GetCustomCode("category", s.cat1)
                        , s.cat2
                        , s.cat3
                        , s.contentid
                        , s.contenttypeid
                        , s.createdtime
                        , s.eventstartdate
                        , s.eventenddate
                        , s.firstimage
                        , s.firstimage2
                        , s.mapx
                        , s.mapy
                        , s.mlevel
                        , s.modifiedtime
                        , s.readcount
                        , areaCode = _fstvlService.GetCustomCode("area", s.areacode)
                        , s.sigungucode
                        , s.tel
                        , s.title
                        , status = this.FestivalStatus(s)
                    }
                )
            };

            return new JsonResult(json);
        }

        private string FestivalStatus(FestivalItem item)
        {
            var status = "";
            var today = Convert.ToInt32(DateTime.Now.ToString("yyyyMMdd"));

            var st = Convert.ToInt32(item.eventstartdate);
            var et = Convert.ToInt32(item.eventenddate);

            if(today < st)
            {
                status = "행사대기";
            }
            else if(today >= st && today <= et)
            {
                status = "행사중";
            }
            else if(today > et)
            {
                status = "행사종료";
            }

            return status;
        }


        [HttpPost]
        public IActionResult AreaCode(FestivalReqModel req)
        {
            req.serviceKey = _configuration.GetSection("OPENAPI:PUBLIC_API_KEY").Value;
            req.searchurl = _configuration.GetSection("OPENAPI:FESTIVAL_API_AREACODE_URL").Value;
            req.pageNo = 1;
            req.numOfRows = 1000;
            req.MobileOS = "ETC";
            req.MobileApp = "MapView";
            req._type = "json";


            var response = _fstvlService.GetAreaCode< AreaCodeModel>(req);


            return new JsonResult(response);
        }


        [HttpPost]
        public IActionResult Detail(FestivalDetailReqModel req)
        {
            var model = new FestivalDetailCommonRes();

            req.serviceKey = _configuration.GetSection("OPENAPI:PUBLIC_API_KEY").Value;
            req.searchurl = _configuration.GetSection("OPENAPI:FESTIVAL_API_DETAILCOMMON_URL").Value;
            req.MobileOS = "ETC";
            req.MobileApp = "MapView";
            req._type = "json";
            req.contentId = req.contentId;

            model = _fstvlService.GetFestivalDetail<FestivalDetailCommonRes>(req);

            foreach (var i in model.response.body.items.item)
            {
                i.cat1 = _fstvlService.GetCustomCode("category", i.cat1);
                i.areacode = _fstvlService.GetCustomCode("area", i.areacode);
            }

            return new JsonResult(model);
        }


        [HttpPost]
        public IActionResult DetailInfo(FestivalDetailReqModel req)
        {


            req.serviceKey = _configuration.GetSection("OPENAPI:PUBLIC_API_KEY").Value;
            req.searchurl = _configuration.GetSection("OPENAPI:FESTIVAL_API_DETAILINFO_URL").Value;
            req.MobileOS = "ETC";
            req.MobileApp = "MapView";
            req._type = "json";
            req.contentId = req.contentId;
            req.contentTypeId = req.contentTypeId;

            var model  = _fstvlService.GetFestivalDetail<FestivalDetailInfoRes>(req);

            return new JsonResult(model);
        }


        [HttpPost]
        public IActionResult DetailImage(FestivalDetailReqModel req)
        {
            req.serviceKey = _configuration.GetSection("OPENAPI:PUBLIC_API_KEY").Value;
            req.searchurl = _configuration.GetSection("OPENAPI:FESTIVAL_API_DETAILIMAGE_URL").Value;
            req.MobileOS = "ETC";
            req.MobileApp = "MapView";
            req._type = "json";
            req.contentId = req.contentId;
            req.contentTypeId = req.contentTypeId;

            var model = _fstvlService.GetFestivalDetail<FestivalDetailImageRes>(req);

            return new JsonResult(model);
        }


        [HttpPost]
        public IActionResult Blogs(string query)
        {
            var response = _fstvlService.GetBlogList(query);

            return new JsonResult(response);
        }
    }
}
