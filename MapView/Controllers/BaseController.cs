using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;



namespace MapView.Controllers
{
    public class BaseController : Controller
    {

        public BaseController()
        {
            
        }

        public string GetUserId()
        {
            string UserId = string.Empty;

            if (User != null)
            {
                if (User.Identity.IsAuthenticated)
                {
                    UserId = User.Claims.FirstOrDefault(x => x.Type == "sns").Value + "_" + User.Claims.FirstOrDefault(x => x.Type == "id").Value;
                }
            }
            
            return UserId;
        }
    }
}
