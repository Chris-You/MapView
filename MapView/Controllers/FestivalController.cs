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
using MapView.Common.Models;

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

            var json = this.RetJson(response);

            return new JsonResult(json);
        }

        private object RetJson(FestivalResModel response)
        {
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

            return json;
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


        [HttpPost]
        public IActionResult FavorList()
        {
            var list = new List<Favor>();
            if (string.IsNullOrEmpty(base.GetUserId()) == false)
            {
                list = _userService.FavorList(base.GetUserId(), ServiceGubun.festival);


                Parallel.ForEach(list, i => {

                    Parallel.Invoke(
                       () => {
                           i.festival.cat1 = _fstvlService.GetCustomCode("category", i.festival.cat1);
                           i.festival.areacode = _fstvlService.GetCustomCode("category", i.festival.areacode);
                           i.festival.status = this.FestivalStatus(i.festival);
                       }
                    );
                });
            }
            
            return new JsonResult(list);
        }


        [HttpPost]
        public IActionResult Favor(string contentid, bool isFavor)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(base.GetUserId()) == false)
            {
                var isOk = false;
                var msg = "";

                if (isFavor)
                {
                    // 삭제
                    isOk = _userService.DelFavor(base.GetUserId(), ServiceGubun.festival, contentid);
                }
                else
                {

                    FestivalReqModel req = new FestivalReqModel();
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

                    Favor favor = new Favor();
                    favor.contentId = contentid;
                    favor.date = DateTime.Now;
                    favor.user = base.GetUserId();
                    favor.service = ServiceGubun.festival;
                    favor.festival = response.response.body.items.item.Where(w => w.contentid == contentid).FirstOrDefault();

                    // 등록
                    isOk = _userService.InsFavor(favor); // base.GetUserId(), ServiceGubun.camp, contentid, "");
                    
                }
                if (isOk)
                {
                    msg = "ok";
                }

                dic.Add("result", isOk.ToString());
                dic.Add("message", msg);
            }
            else
            {
                dic.Add("result", "False");
                dic.Add("message", "Require Login");
            }

            return new JsonResult(dic);
        }

        [HttpPost]
        public IActionResult IsFavor(string contentid)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(base.GetUserId()) == false)
            {
                var favor = new Favor();
                favor.user = base.GetUserId();
                favor.contentId = contentid;
                favor.service = ServiceGubun.festival;

                var isOk = _userService.ChkFavor(favor);

                dic.Add("result", isOk.ToString());
                dic.Add("message", "ok");
            }
            else
            {
                dic.Add("result", "False");
                dic.Add("message", "false");
            }

            return new JsonResult(dic);
        }

    }
}
