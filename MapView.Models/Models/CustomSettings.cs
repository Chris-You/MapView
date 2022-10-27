using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapView.Common.Models.CustomSettings
{
    public class ChargerCode
    {
        // 충전기 상태
        public List<Value> stat { get; set; }

        // 충전기 타입
        public List<Value> chgerType { get; set; }

        // 지역코드
        public List<Value> zcode { get; set; }

        //지역코드 상세
        public List<Value> zscode { get; set; }

        //충전소 구분
        public List<Value> kind { get; set; }

        //충전소 구분 상새
        public List<Value> kindDetail { get; set; }

        // 기관아이디
        public List<Value> busid { get; set; }
    }

    public class Value
    {
        public string code { get; set; }
        public string name { get; set; }
    }



    public enum CodeType
    {
        stat,
        chgerType,
        zcode,
        zscode,
        kind,
        kindDetail,
        busid

    }


    public class FestivalCode
    {
        public List<itemValue> areaCode { get; set; }

        public List<itemValue> categoryCode { get; set; }
    }

    public class itemValue
    {
        public string rnum { get; set; }
        public string code { get; set; }
        public string name { get; set; }
        
    }
}
