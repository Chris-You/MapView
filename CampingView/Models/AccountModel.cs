using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampView.Models
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


    public class KakaoAccessToken
    {
        public string token_type { get; set; }
        public string id_token { get; set; }
        public string access_token { get; set; }
        public int expires_in { get; set; }
        public string refresh_token { get; set; }
        public int refresh_token_expires_in { get; set; }
        public string scope { get; set; }
    }

    public class KakaoUserProfile
    {
        public string id { get; set; }
        public string connected_at { get; set; }
        public KakaoAccount kakao_account { get; set; }
    }

    public class KakaoAccount
    {
        public bool profile_nickname_needs_agreement { get; set; }
        public bool profile_image_needs_agreement { get; set; }

        public KakaoProfile profile { get; set; }
        public bool name_needs_agreement { get; set; }
        public string name { get; set; }
        public bool email_needs_agreement { get; set; }
        public bool is_email_valid { get; set; }
        public bool is_email_verified { get; set; }
        public string email { get; set; }
        public bool age_range_needs_agreement { get; set; }
        public string age_range { get; set; }
        public bool birthyear_needs_agreement { get; set; }
        public string birthyear { get; set; }
        public bool birthday_needs_agreement { get; set; }
        public string birthday { get; set; }
        public string birthday_type { get; set; }
        public bool gender_needs_agreement { get; set; }
        public string gender { get; set; }
        public bool phone_number_needs_agreement { get; set; }
        public string phone_number { get; set; }
        public bool ci_needs_agreement { get; set; }
        public string ci { get; set; }


    }

    public class KakaoProfile
    {
        public string nickname { get; set; }
        public string thumbnail_image_url { get; set; }
        public string profile_image_url { get; set; }
        public bool is_default_image { get; set; }
    }


}
