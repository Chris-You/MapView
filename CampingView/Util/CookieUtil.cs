using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;

namespace CampView.Util
{
    public class CookieUtil
    {
        public void SetCookie(string name, string value, int? expireDay)
        {
            Cookie cookie = new Cookie(name, value);

            if(expireDay != null && expireDay > 0)
            cookie.Expires = DateTime.Now.AddDays((int)expireDay);

            
        }

        public void GetCookie()
        {

        }

        public void DeleteCookie()
        {

        }
    }
}
