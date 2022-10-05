using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CampView.Models;
using CampView.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;

namespace CampView.Controllers
{
    public class LottoController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CampController> _logger;
        private readonly ILottoService _lottoService;

        public LottoController(ILogger<CampController> logger, IConfiguration config,
                             ILottoService lottoService)
        {
            _logger = logger;
            _configuration = config;
            _lottoService = lottoService;
        }


        public IActionResult Index()
        {
            return View();
        }


        [HttpGet]
        public  IActionResult SetLottoData(int idx, int start, int end)
        {
            bool response = false;
            

            if (start > 0 && end > 0 && end > start)
            {
                response = _lottoService.SetLottoData(start, end);
            }
            else
            {
                //response = _lottoService.SetLottoData(idx);
            }

            return new JsonResult(response);
        }

        [HttpPost]
        public IActionResult Prize(string date)
        {
            var response = new LottoResModel();

            response = _lottoService.GetPrize(date);

            return new JsonResult(response);
        }

    }
}
