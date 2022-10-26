using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MapView.Common.Models;
using MapView.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

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
            req.searchurl = _configuration.GetSection("OPENAPI:FSTVL_API_URL").Value;
            req.pageNo = 1;
            req.numOfRows = 1000;
            req.userid = base.GetUserId();

            var response = _fstvlService.GetFestivalList(req);


            return new JsonResult(response);
        }

    }
}
