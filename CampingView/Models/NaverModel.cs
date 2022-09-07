using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampingView.Models
{
    #region ////네이버 블로그 검색
    public class SearchResModel
    {
        public int total { get; set; }
        public int start { get; set; }
        public int display { get; set; }

        public List<item> items { get; set; }
    }


    public class item
    {
        public string title { get; set; }
        public string link { get; set; }
        public string description { get; set; }
        public string bloggername { get; set; }
        public string bloggerlink { get; set; }
        public string postdate { get; set; }
    }

    #endregion




    public class NaverAccessToken
    {
        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
    }


    public class NaverUserProfile
    {
        public string resultcode { get; set; }
        public string message { get; set; }
        
        public NaverProfile response { get; set; }
    }

    public class NaverProfile
    {
        public string id { get; set; }
        public string nickname { get; set; }
        public string name { get; set; }
        public string email { get; set; }
        public string gender { get; set; }
        public string age { get; set; }
        public string birthday { get; set; }
        public string birthyear { get; set; }
        public string profile_image { get; set; }
        public string mobile { get; set; }
    }
}
