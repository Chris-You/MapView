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
            req.numOfRows = 999;
            req.userid = base.GetUserId();

            req.zcode = _chargerService.GetZcode(req);
            req.zscode = _chargerService.GetZScode(req);

            // 근처 충전소 리스트
            var itemList = _chargerService.GetChargerList(req);
            //var statusList = _chargerService.GetChargerList(req);
            var chgr = new List<ChargerItem>();
            var status = new List<ChargerStatusItem>();

            

            Parallel.Invoke(
                ()=> {
                    chgr = _chargerService.GetChargerAPIList(req);
                },
                () => {
                    //status = _chargerService.GetChargerStatusAPIList(req);
                }
            );


            Parallel.ForEach(itemList, i => {
                
                Parallel.Invoke(
                   () => {
                       i.chgr = chgr.Where(w => w.statId == i.statId).ToList();
                   },
                   () => {
                       //i.status = status.Where(w => w.statId == i.statId).ToList();
                   }
               );

            });

            

            // 실시간충전소 현황 API




            //var model = new ChargerModel();
            /*
                foreach (var itm in list)
                {
                    var i = new Item();
                    i.statNm = itm.statNm;
                    i.statId = itm.statId;
                    i.addr = itm.addr;
                    i.location = itm.location;
                    i.lat = itm.lat;
                    i.lng = itm.lng;

                
                    foreach (var dtl in itemList.Where(w => w.statId == itm.statId))
                    {
                        var d = new ItemDetail();
                        d.chgerId = dtl.chgerId;
                        d.chgerType = dtl.chgerType;
                        d.useTime = dtl.useTime;
                        d.busiId = dtl.busiId;
                        d.bnm = dtl.bnm;
                        d.stat = dtl.stat;
                        d.statUpdDt = dtl.statUpdDt;
                        d.lastTsdt = dtl.lastTsdt;
                        d.lastTedt = dtl.lastTedt;
                        d.nowTsdt = dtl.nowTsdt;
                        d.output = dtl.output;
                        d.delYn = dtl.delYn;
                        d.delDetail = dtl.delDetail;
                        d.busiNm     = dtl.busiNm;
                        d.busiCall   = dtl.busiCall;
                        d.powerType  = dtl.powerType;
                        d.zcode      = dtl.zcode;
                        d.zscode     = dtl.zscode;
                        d.kind       = dtl.kind;
                        d.kindDetail = dtl.kindDetail;
                        d.parkingFree= dtl.parkingFree;
                        d.note       = dtl.note;
                        d.limitYn    = dtl.limitYn;
                        d.limitDetail =dtl.limitDetail;

                        i.charger.Add(d);
                    }
                

                    model.item.Add(i);

                }
                */

            var val = _chargerConfig.Value;

            //todo  filter
            //response = _chargerService.CampFilter(req, response);

            return new JsonResult(itemList.OrderBy(s => s.statNm));
        }
    }
}
