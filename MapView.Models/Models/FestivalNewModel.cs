using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapView.Common.Models.Festival
{

    #region /////데이터 리스트 모델

    public class FestivalReqModel
    {
        public string userid { get; set; }
        public string searchurl { get; set; }       
        
        public string serviceKey { get; set; }
        public int pageNo { get; set; }
        public int numOfRows { get; set; }

        public string MobileOS { get; set; }        //OS 구분 : IOS (아이폰), AND (안드로이드), WIN (윈도우폰), ETC(기타)
        public string MobileApp { get; set; }       //서비스명(어플명)
        public string _type { get; set; }

        public string listYN { get; set; }      //목록구분(Y=목록, N=개수)
        public string arrange { get; set; }     //정렬구분 (A=제목순, C=수정일순, D=생성일순) 대표이미지가반드시있는정렬(O=제목순, Q=수정일순, R=생성일순)
        public string areaCode { get; set; }
        public string sigunguCode { get; set; }
        public string eventStartDate { get; set; }      //행사시작일(형식 :YYYYMMDD)
        public string eventEndDate { get; set; }        //행사종료일(형식 :YYYYMMDD)

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
        public FestivalItems items { get; set; }
        public int numOfRows { get; set; }
        public int pageNo { get; set; }
        public int totalCount { get; set; }

        public FestivalBody()
        {
            items = new FestivalItems();
        }
    }

    public class FestivalItems
    {
        public List<FestivalItem> item { get; set; }
    }


    public class FestivalItem
    {
        
        public string addr1 { get; set; }
        public string addr2 { get; set; }
        public string booktour { get; set; }    //교과서속여행지 여부
        public string cat1 { get; set; }
        public string cat2 { get; set; }
        public string cat3 { get; set; }
        public string contentid { get; set; }
        public string contenttypeid { get; set; }
        public string createdtime { get; set; }
        public string eventstartdate { get; set; }
        public string eventenddate { get; set; }
        public string firstimage { get; set; }      //대표이미지(원본)
        public string firstimage2 { get; set; }     //대표이미지(썸네일)
        public string mapx { get; set; }
        public string mapy { get; set; }
        public string mlevel { get; set; }          //Map Level
        public string modifiedtime { get; set; }

        public string readcount { get; set; }
        public string areacode { get; set; }
        public string sigungucode { get; set; }
        public string tel { get; set; }
        public string title { get; set; }


    }



    public class AreaCodeModel
    {

    }


    public class FestivalDetailReqModel
    {
        public string userid { get; set; }
        public string searchurl { get; set; }

        public string serviceKey { get; set; }
        public string MobileOS { get; set; }        //OS 구분 : IOS (아이폰), AND (안드로이드), WIN (윈도우폰), ETC(기타)
        public string MobileApp { get; set; }       //서비스명(어플명)
        public string _type { get; set; }

        public string contentId { get; set; }    
        public string contentTypeId { get; set; }
        public string defaultYN { get; set; }
        public string firstImageYN { get; set; }
        public string areacodeYN { get; set; }
        public string catcodeYN { get; set; }

        public string addrinfoYN { get; set; }
        public string mapinfoYN { get; set; }
        public string overviewYN { get; set; }
    }

    /*
    public class FestivalDetailResModel
    {
        public FestivalDetailCommonRes detailCommon { get; set; }

        public FestivalDetailInfoRes detailInfo { get; set; }


        public FestivalDetailImageRes detailImage { get; set; }
    }
    */


    public class FestivalDetailCommonRes
    {
        public FestivalDetailResponse response { get; set; }

        public FestivalDetailCommonRes()
        {
            response = new FestivalDetailResponse();
        }
    }


    public class FestivalDetailResponse
    {
        public FestivalHeader header { get; set; }
        public FestivalDetailCommonBody body { get; set; }

        public FestivalDetailResponse()
        {
            header = new FestivalHeader();
            body = new FestivalDetailCommonBody();
        }
    }

    public class FestivalDetailCommonBody
    {
        public FestivalDetailCommonItems items { get; set; }
        public int numOfRows { get; set; }
        public int pageNo { get; set; }
        public int totalCount { get; set; }

        public FestivalDetailCommonBody()
        {
            items = new FestivalDetailCommonItems();
        }
    }

    public class FestivalDetailCommonItems
    {
        public List<FestivalDetailCommonItem> item { get; set; }
    }


    public class FestivalDetailCommonItem
    {
        public string contenttypeid { get; set; }
        public string contentid { get; set; }
        public string booktour { get; set; }    //교과서속여행지 여부
        public string createdtime { get; set; }
        public string tel { get; set; }
        public string telname { get; set; }
        public string homepage { get; set; }
        public string title { get; set; }
        public string firstimage { get; set; }      //대표이미지(원본)
        public string firstimage2 { get; set; }     //대표이미지(썸네일)
        public string areacode { get; set; }
        public string sigungucode { get; set; }
        public string zipcode { get; set; }
        public string addr1 { get; set; }
        public string addr2 { get; set; }
        
        public string cat1 { get; set; }
        public string cat2 { get; set; }
        public string cat3 { get; set; }
        public string mapx { get; set; }
        public string mapy { get; set; }
        public string mlevel { get; set; }          //Map Level
        public string overview { get; set; }
    }




    public class FestivalDetailInfoRes
    {
        public FestivalDetailInfoResponse response { get; set; }

        public FestivalDetailInfoRes()
        {
            response = new FestivalDetailInfoResponse();
        }
    }


    public class FestivalDetailInfoResponse
    {
        public FestivalHeader header { get; set; }
        public FestivalDetailInfoBody body { get; set; }

        public FestivalDetailInfoResponse()
        {
            header = new FestivalHeader();
            body = new FestivalDetailInfoBody();
        }
    }

    public class FestivalDetailInfoBody
    {
        public FestivalDetailInfoItems items { get; set; }
        public int numOfRows { get; set; }
        public int pageNo { get; set; }
        public int totalCount { get; set; }

        public FestivalDetailInfoBody()
        {
            items = new FestivalDetailInfoItems();
        }
    }

    public class FestivalDetailInfoItems
    {
        public List<FestivalDetailInfoItem> item { get; set; }
    }

    public class FestivalDetailInfoItem
    {
        public string roomimg4alt {get;set;}
        public string roomimg5 {get;set;}
        public string roomimg5alt {get;set;}
        public string contentid {get;set;}
        public string contenttypeid {get;set;}
        public string fldgubun {get;set;}
        public string infoname {get;set;}
        public string infotext {get;set;}
        public string serialnum {get;set;}
        public string subcontentid {get;set;}
        public string subdetailalt {get;set;}
        public string subdetailimg {get;set;}
        public string subdetailoverview {get;set;}
        public string subname {get;set;}
        public string subnum {get;set;}
        public string roomcode {get;set;}
        public string roomtitle {get;set;}
        public string roomsize1 {get;set;}
        public string roomcount {get;set;}
        public string roombasecount {get;set;}
        public string roommaxcount {get;set;}
        public string roomoffseasonminfee1 {get;set;}
        public string roomoffseasonminfee2 {get;set;}
        public string roompeakseasonminfee1 {get;set;}
        public string roompeakseasonminfee2 {get;set;}
        public string roomintro {get;set;}
        public string roombathfacility {get;set;}
        public string roombath {get;set;}
        public string roomhometheater {get;set;}
        public string roomaircondition {get;set;}
        public string roomtv {get;set;}
        public string roompc {get;set;}
        public string roomcable {get;set;}
        public string roominternet {get;set;}
        public string roomrefrigerator {get;set;}
        public string roomtoiletries {get;set;}
        public string roomsofa {get;set;}
        public string roomcook {get;set;}
        public string roomtable {get;set;}
        public string roomhairdryer {get;set;}
        public string roomsize2 {get;set;}
        public string roomimg1 {get;set;}
        public string roomimg1alt {get;set;}
        public string roomimg2 {get;set;}
        public string roomimg2alt {get;set;}
        public string roomimg3 {get;set;}
        public string roomimg3alt {get;set;}
        public string roomimg4 { get; set; }
    }


    public class FestivalDetailImageRes
    {
        public FestivalDetailImageResponse response { get; set; }

        public FestivalDetailImageRes()
        {
            response = new FestivalDetailImageResponse();
        }
    }


    public class FestivalDetailImageResponse
    {
        public FestivalHeader header { get; set; }
        public FestivalDetailImageBody body { get; set; }

        public FestivalDetailImageResponse()
        {
            header = new FestivalHeader();
            body = new FestivalDetailImageBody();
        }
    }

    public class FestivalDetailImageBody
    {
        public FestivalDetailImageItems items { get; set; }
        public int numOfRows { get; set; }
        public int pageNo { get; set; }
        public int totalCount { get; set; }

        public FestivalDetailImageBody()
        {
            items = new FestivalDetailImageItems();
        }
    }

    public class FestivalDetailImageItems
    {
        public List<FestivalDetailImageItem> item { get; set; }
    }
    public class FestivalDetailImageItem
    {
        public string contentid { get; set; }
        public string originimgurl { get; set; }
        public string imgname { get; set; }
        public string smallimageurl { get; set; }
        public string serialnum { get; set; }
    }

    #endregion



}
