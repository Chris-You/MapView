using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapView.Common.Models
{

    #region /////데이터 리스트 모델

    public class FestivalReqModel
    {
        public string userid { get; set; }
        public string searchurl { get; set; }       
        
        public string serviceKey { get; set; }
        public int pageNo { get; set; }
        public int numOfRows { get; set; }
        
        public string latitude { get; set; }
        public string longitude { get; set; }

        public string keyword { get; set; }
    }


    public class FestivalResModel
    {
        public FestivalResponse response { get; set; }

        public FestivalResModel()
        {
            response = new FestivalResponse();
        }
    }

    public class FestivalResponse
    {
        public FestivalHeader header { get; set; }
        public FestivalBody body { get; set; }

        public FestivalResponse()
        {
            header = new FestivalHeader();
            body = new FestivalBody();
        }

    }

    public class FestivalHeader
    {
        public string resultCode { get; set; }
        public string resultMsg { get; set; }
    }

    public class FestivalBody
    {
        public List<FestivalItem> items { get; set; }
        public int numOfRows { get; set; }
        public int pageNo { get; set; }
        public int totalCount { get; set; }

        public FestivalBody()
        {
            items = new List<FestivalItem>();
        }
    }


    public class FestivalItem
    {
        
        public string fstvlNm { get; set; }
        public string opar { get; set; }
        public string fstvlStartDate { get; set; }
        public string fstvlEndDate { get; set; }
        public string fstvlCo { get; set; }
        public string mnnst { get; set; }
        public string auspcInstt { get; set; }
        public string suprtInstt { get; set; }
        public string phoneNumber { get; set; }
        public string homepageUrl { get; set; }
        public string relateInfo { get; set; }
        public string rdnmadr { get; set; }
        public string lnmadr { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string referenceDate { get; set; }
        public string insttCode { get; set; }
        
    }



    #endregion


    
}
