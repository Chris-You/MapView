using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampView.Models
{

    #region /////데이터 리스트 모델

    public class CampReqModel
    {
        public string userid { get; set; }
        public string searchurl { get; set; }       
        
        public string serviceKey { get; set; }
        public int pageNo { get; set; }
        public int numOfRows { get; set; }
        public string MobileOS { get; set; }
        public string MobileApp { get; set; }

        public string mapX { get; set; }
        public string mapY { get; set; }
        public int radius { get; set; }

        public string keyword { get; set; }     // urlencode
        public string contentId { get; set; }

        public string[] facltDivNm { get; set; }
        public string[] induty { get; set; }
        public string[] sbrsCl { get; set; }
        

    }


    public class CampResModel
    {
        public CampResponse response { get; set; }

        public CampResModel()
        {
            response = new CampResponse();
        }
    }

    public class CampResponse
    {
        public CampHeader header { get; set; }
        public CampBody body { get; set; }

        public CampResponse()
        {
            header = new CampHeader();
            body = new CampBody();
        }

    }

    public class CampHeader
    {
        public string resultCode { get; set; }
        public string resultMsg { get; set; }
    }

    public class CampBody
    {
        public CampItems items { get; set; }
        public int numOfRows { get; set; }
        public int pageNo { get; set; }
        public int totalCount { get; set; }

        public CampBody()
        {
            items = new CampItems();
        }
    }

    public class CampItems
    {
        public List<CampItem> item { get; set; }

        public CampItems()
        {
            item = new List<CampItem>();
        }
    }

    public class CampItem
    {
        
        public string contentId { get; set; }
        public string facltNm { get; set; }
        public string lineIntro { get; set; }
        public string intro { get; set; }
        public string allar { get; set; }
        public string insrncAt { get; set; }
        public string trsagntNo { get; set; }
        public string bizrno { get; set; }
        public string facltDivNm { get; set; }
        public string mangeDivNm { get; set; }
        public string mgcDiv { get; set; }
        public string manageSttus { get; set; }
        public string hvofBgnde { get; set; }
        public string hvofEnddle { get; set; }
        public string featureNm { get; set; }
        public string induty { get; set; }
        public string lctCl { get; set; }
        public string doNm { get; set; }
        public string sigunguNm { get; set; }
        public string zipcode { get; set; }
        public string addr1 { get; set; }
        public string addr2 { get; set; }
        public string mapX { get; set; }
        public string mapY { get; set; }
        public string direction { get; set; }
        public string tel { get; set; }
        public string homepage { get; set; }
        public string resveUrl { get; set; }
        public string resveCl { get; set; }
        public string manageNmpr { get; set; }
        public string gnrlSiteCo { get; set; }
        public string autoSiteCo { get; set; }
        public string glampSiteCo { get; set; }
        public string caravSiteCo { get; set; }
        public string indvdlCaravSiteCo { get; set; }
        public string sitedStnc { get; set; }
        public string siteMg1Width { get; set; }
        public string siteMg2Width { get; set; }
        public string siteMg3Width { get; set; }
        public string siteMg1Vrticl { get; set; }
        public string siteMg2Vrticl { get; set; }
        public string siteMg3Vrticl { get; set; }
        public string siteMg1Co { get; set; }
        public string siteMg2Co { get; set; }
        public string siteMg3Co { get; set; }
        public string siteBottomCl1 { get; set; }
        public string siteBottomCl2 { get; set; }
        public string siteBottomCl3 { get; set; }
        public string siteBottomCl4 { get; set; }
        public string siteBottomCl5 { get; set; }
        public string tooltip { get; set; }
        public string glampInnerFclty { get; set; }
        public string caravInnerFclty { get; set; }
        public string prmisnDe { get; set; }
        public string operPdCl { get; set; }
        public string operDeCl { get; set; }
        public string trlerAcmpnyAt { get; set; }
        public string caravAcmpnyAt { get; set; }
        public string toiletCo { get; set; }
        public string swrmCo { get; set; }
        public string wtrplCo { get; set; }
        public string brazierCl { get; set; }
        public string sbrsCl { get; set; }
        public string sbrsEtc { get; set; }
        public string posblFcltyCl { get; set; }
        public string posblFcltyEtc { get; set; }
        public string clturEventAt { get; set; }
        public string clturEvent { get; set; }
        public string exprnProgrmAt { get; set; }
        public string exprnProgrm { get; set; }
        public string extshrCo { get; set; }
        public string frprvtWrppCo { get; set; }
        public string frprvtSandCo { get; set; }
        public string fireSensorCo { get; set; }
        public string themaEnvrnCl { get; set; }
        public string eqpmnLendCl { get; set; }
        public string animalCmgCl { get; set; }
        public string tourEraCl { get; set; }
        public string firstImageUrl { get; set; }
        public string createdtime { get; set; }
        public string modifiedtime { get; set; }

        public List<Facility> facility { get; set; }
    }

    public class Facility
    {
        public string name { get; set; }
        public string url { get; set; }
    }

    #endregion

    #region /////이미지 조회 모델

    public class CampImgResModel
    {
        public CampImgResponse response { get; set; }
    }

    public class CampImgResponse
    {
        public CampImgHeader header { get; set; }
        public CampImgBody body { get; set; }
    }

    public class CampImgHeader
    {
        public string resultCode { get; set; }
        public string resultMsg { get; set; }
    }



    public class CampImgBody
    {
        public CampImgItems items { get; set; }
        public int numOfRows { get; set; }
        public int pageNo { get; set; }
        public int totalCount { get; set; }

        public CampImgBody()
        {
            items = new CampImgItems();
        }
    }

    public class CampImgItems
    {
        public List<CampImgItem> item { get; set; }

        public CampImgItems()
        {
            item = new List<CampImgItem>();
        }

    }

    public class CampImgItem
    {
        public string contentId { get; set; }
        public string serialnum { get; set; }
        public string imageUrl { get; set; }
        public string imageUrlOri { get; set; }

        public string createdtime { get; set; }
        public string modifiedtime { get; set; }
    }

    #endregion


    public class  CampComment
    {
        public string user { get; set; }
        public string contentid { get; set; }
        public string comment { get; set; }
        public string ymd { get; set; }
        public bool me { get; set; }
    }
    
}
