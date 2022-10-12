using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CampView.Models.Charger;
using CampView.Models;
using CampView.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using CampView.Models.CustomSettings;

namespace CampView.Controllers
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
            return View();
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
            //var statusList = _chargerService.GetChargerList(req);
            //var chgr = new List<ChargerItem>();
            var chgr = _chargerService.GetChargerAPIList(req, itemList);
            //var status = new List<ChargerStatusItem>();


            
            Parallel.ForEach(itemList, i => {
                
                Parallel.Invoke(
                   () => {
                       i.chgr = chgr.Where(w => w.statId == i.statId).ToList();
                   },
                   () => {
                       //i.status = status.Where(w => w.statId == i.statId).ToList();
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
        public IActionResult Comments()
        {
            var list = _chargerService.CommentList().OrderByDescending(o => o.date).Take(10);

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



    }
}
