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
    public class BaseController : Controller
    {
        public BaseController()
        {
        }
    }
}
