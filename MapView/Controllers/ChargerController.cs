using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MapView.Models.Charger;
using MapView.Models;
using MapView.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using MapView.Models.CustomSettings;

namespace MapView.Controllers
{
    public class ChargerController : BaseController
    {

        private readonly IConfiguration _configuration;
        private readonly ILogger<ChargerController> _logger;

        private IChargerService _chargerService;
        private IAccountService _accountService;
        private IUserService _userService;

        private readonly IOptions<ChargerCode> _chargerConfig;

        public ChargerController(ILogger<ChargerController> logger, IConfiguration config,
                      IChargerService chargerService, IAccountService accountService,
                      IUserService userService,
                      IOptions<ChargerCode> chargerConfig)
        {
            _logger = logger;
            _configuration = config;
            _chargerConfig = chargerConfig;
            _chargerService = chargerService;
            _accountService = accountService;
            _userService = userService;
        }





        public IActionResult Index()
        {
            ViewBag.UserId = base.GetUserId();

            return View();
        }


        /// <summary>
        /// 충전소 기본정보 json 파일 생성(매주 일요일 새벽 1회 실행이 되도록)
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult MakeFile(ChargerReqModel req)
        {
            req.serviceKey = _configuration.GetSection("OPENAPI:PUBLIC_API_KEY").Value;
            req.searchurl = _configuration.GetSection("OPENAPI:CHARGER_API_INFO_URL").Value;
            req.pageNo = 1;
            req.numOfRows = 9999;
            req.userid = base.GetUserId();

            //req.zcode = _chargerService.GetZcode(req);
            //req.zscode = _chargerService.GetZScode(req);

            _chargerService.MakeFile(req);

            var json = new
            {
                result = "ok",
            };


            return new JsonResult(json);
        }

        /// <summary>
        /// 10분마다 호출 하여 데이터 캐싱해놓기
        /// </summary>
        /// <param name="req"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult MakeRedisCache(ChargerReqModel req)
        {
            req.serviceKey = _configuration.GetSection("OPENAPI:PUBLIC_API_KEY").Value;
            req.searchurl = _configuration.GetSection("OPENAPI:CHARGER_API_INFO_URL").Value;
            
            req.numOfRows = 9999;
            req.userid = base.GetUserId();

            //req.zcode = _chargerService.GetZcode(req);
            //req.zscode = _chargerService.GetZScode(req);

            if (string.IsNullOrEmpty(req.zcode) == false)
            {
                var zscode = _chargerConfig.Value.zscode;

                foreach (var zs in zscode.Where(w=> w.code.Substring(0,2) == req.zcode))
                {
                    req.pageNo = 1;
                    req.zcode = zs.code.Substring(0, 2);
                    req.zscode = zs.code;

                    var list = _chargerService.MakeRedisCache(req);
                }
            }
            else
            {
                var list = _chargerService.MakeRedisCache(req);
            }

            var json = new
            {
                result = "ok",
            };

            return new JsonResult(json);
        }



        [HttpPost]
        public IActionResult Search(ChargerReqModel req)
        {
            req.serviceKey = _configuration.GetSection("OPENAPI:PUBLIC_API_KEY").Value;
            req.searchurl = _configuration.GetSection("OPENAPI:CHARGER_API_INFO_URL").Value;
            req.pageNo = 1;
            req.numOfRows = 9999;
            req.userid = base.GetUserId();

            req.zcode = _chargerService.GetZcode(req);
            req.zscode = _chargerService.GetZScode(req);

            // 근처 충전소 리스트
            var itemList = _chargerService.GetChargerList(req);
            var chgr = _chargerService.GetChargerDtlList(req);
            
            
            Parallel.ForEach(itemList, i => {
                
                Parallel.Invoke(
                   () => {
                       i.chgr = chgr.Where(w => w.statId == i.statId).ToList();
                   }
                );


                i.avail = i.chgr.Where(w => w.stat == "2" && w.delYn == "N").Count() > 0 ? "Y" :
                                    (i.chgr.Where(w => w.stat == "0" || w.stat == "1" || w.stat == "3").Count() == i.chgr.Count() ? "X" : "N");
                                        

                i.kindNm = _chargerService.GetCodeNm(CodeType.kind, i.kind);
                i.kindDetailNm = _chargerService.GetCodeNm(CodeType.kindDetail, i.kindDetail);
            });
            
            

            

            //todo  filter
            //response = _chargerService.CampFilter(req, response);

            return new JsonResult(itemList.OrderBy(s => s.statNm));
        }


        [HttpPost]
        public IActionResult SearchDtl(string statId, string zscode)
        {
            ChargerReqModel req = new ChargerReqModel();

            req.serviceKey = _configuration.GetSection("OPENAPI:PUBLIC_API_KEY").Value;
            req.searchurl = _configuration.GetSection("OPENAPI:CHARGER_API_INFO_URL").Value;
            req.pageNo = 1;
            req.numOfRows = 9999;
            req.userid = base.GetUserId();
            req.zcode = zscode.Substring(0,2);
            req.zscode = zscode;


            // 근처 충전소 리스트
            var itemList = _chargerService.GetChargerList(req);
            itemList = itemList.Where(w => w.statId == statId).ToList();
            var chgr = _chargerService.GetChargerDtlList(req);
            

            Parallel.ForEach(itemList, i => {

                Parallel.Invoke(
                   () => {
                       i.chgr = chgr.Where(w => w.statId == i.statId).ToList();
                   }
                );

                i.avail = i.chgr.Where(w => w.stat == "2" && w.delYn == "N").Count() > 0 ? "Y" :
                                    (i.chgr.Where(w => w.stat == "0" || w.stat == "1" || w.stat == "3").Count() == i.chgr.Count() ? "X" : "N");

                i.kindNm = _chargerService.GetCodeNm(CodeType.kind, i.kind);
                i.kindDetailNm = _chargerService.GetCodeNm(CodeType.kindDetail, i.kindDetail);
            });

            return new JsonResult(itemList.OrderBy(s => s.statNm));
        }




        [HttpPost]
        public IActionResult Comments(string statId)
        {
            var list = new List<ChargerComment>();

            if (string.IsNullOrEmpty(statId))
            {
                list = _chargerService.CommentList()?
                    .OrderByDescending(o => o.date).Take(10).ToList();
            }
            else
            {
                list = _chargerService.CommentList(statId);
                if (list != null && list.Count() > 0)
                {
                    foreach (var i in list)
                    {
                        if (i.user == base.GetUserId())
                        {
                            i.me = true;
                        }
                    }

                    list = list.OrderByDescending(o => o.me).OrderByDescending(o => o.date).ToList();
                }
            }

            return new JsonResult(list);

        }


        [HttpPost]
        public IActionResult CommentIns(ChargerComment comm)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(base.GetUserId()) == false)
            {
                comm.user = base.GetUserId();
                comm.date = DateTime.Now;

                comm = _chargerService.CommentIns(comm);

                if(comm != null)
                {
                    dic.Add("result", "true");
                    dic.Add("message", "ok");
                }
                
            }
            else
            {
                dic.Add("result", "False");
                dic.Add("message", "Require Login");
            }

            return new JsonResult(dic);
        }


        [HttpPost]
        public IActionResult CommentDel(ChargerComment comm)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(base.GetUserId()) == false)
            {
                comm.user = base.GetUserId();

                if (_chargerService.DelComment(comm))
                {
                    dic.Add("result", "true");
                    dic.Add("message", "ok");
                }
            }
            else
            {
                dic.Add("result", "False");
                dic.Add("message", "Require Login");
            }

            return new JsonResult(dic);
        }


        [HttpPost]
        public IActionResult FavorList()
        {
            var list = new List<ChargerFavor>();
            if (string.IsNullOrEmpty(base.GetUserId()) == false)
            {
                list = _chargerService.FavorList(base.GetUserId());

                Parallel.ForEach(list, i => {

                    Parallel.Invoke(
                       () => {
                           ChargerReqModel req = new ChargerReqModel();
                           req.serviceKey = _configuration.GetSection("OPENAPI:PUBLIC_API_KEY").Value;
                           req.searchurl = _configuration.GetSection("OPENAPI:CHARGER_API_INFO_URL").Value;
                           req.pageNo = 1;
                           req.numOfRows = 9999;
                           req.userid = base.GetUserId();
                           req.zcode = i.zscode.Substring(0, 2);
                           req.zscode = i.zscode;

                           var chgr = _chargerService.GetChargerDtlList(req);

                           i.totalCnt = chgr.Where(w => w.statId == i.statId).Count();
                           i.availCnt = chgr.Where(w => w.statId == i.statId && w.stat == "2" && w.delYn == "N").Count();
                       }
                    );
                });
            }

            return new JsonResult(list.Where(w => string.IsNullOrEmpty(w.statNm) == false));
        }


            [HttpPost]
        public IActionResult Favor(string statId, string zscode, bool isFavor)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(base.GetUserId()) == false)
            {
                var isOk = false;
                var msg = "";

                if (isFavor)
                {
                    // 삭제
                    isOk = _chargerService.DelFavor(base.GetUserId(), statId);
                }
                else
                {
                    // 등록
                    isOk = _chargerService.InsFavor(base.GetUserId(), statId, zscode);

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
        public IActionResult IsFavor(string statId)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (string.IsNullOrEmpty(base.GetUserId()) == false)
            {
                var isOk = _chargerService.ChkFavor(base.GetUserId(), statId);

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
