using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CampView.Models.Charger
{

    #region /////데이터 리스트 모델

    public class ChargerReqModel
    {
        public string userid { get; set; }
        public string searchurl { get; set; }       
        
        public string serviceKey { get; set; }
        public int pageNo { get; set; }
        public int numOfRows { get; set; }
        public string zcode { get; set; }
        public string zscode { get; set; }

        public string kind { get; set; }
        public string kindDetail { get; set; }
        public string dataType { get; set; }

        public string depth1 { get; set; }
        public string depth2 { get; set; }
        public string depth3 { get; set; }

        public string lat { get; set; }
        public string lng { get; set; }

    }

    /*
    public class ChargerResModel
    {
        public ChargerResponse response { get; set; }

        public ChargerResModel()
        {
            response = new ChargerResponse();
        }
    }*/

    /*
    public class ChargerModel
    {
        public List<Item> item { get; set; }

        public ChargerModel()
        {
            item = new List<Item>();
        }

    }

    public class Item
    {
        public string statNm { get; set; }
        public string statId { get; set; }
        public string addr { get; set; }
        public string location { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        

        public List<ItemDetail> charger { get; set; }
        public Item()
        {
            charger = new List<ItemDetail>();
        }
    }


    public class ItemDetail
    {
        public string chgerId { get; set; }
        public string chgerType { get; set; }
        public string useTime { get; set; }
        public string busiId { get; set; }
        public string bnm { get; set; }
        public string stat { get; set; }
        public string statUpdDt { get; set; }
        public string lastTsdt { get; set; }
        public string lastTedt { get; set; }
        public string nowTsdt { get; set; }
        public string output { get; set; }
        public string delYn { get; set; }
        public string delDetail { get; set; }
        public string busiNm { get; set; }
        public string busiCall { get; set; }
        public string powerType { get; set; }
        public string zcode { get; set; }
        public string zscode { get; set; }
        public string kind { get; set; }
        public string kindDetail { get; set; }
        public string parkingFree { get; set; }
        public string note { get; set; }
        public string limitYn { get; set; }
        public string limitDetail { get; set; }
    }
    */






    public class ChargerResModel
    {
        public string resultCode { get; set; }
        public string resultMsg { get; set; }
        public int numOfRows { get; set; }
        public int pageNo { get; set; }
        public int totalCount { get; set; }

        public ChargerItems items { get; set; }

        public ChargerResModel()
        {
            items = new ChargerItems();
        }
    }



    public class ChargerItems
    {
        public List<ChargerItem> item { get; set; }

        public ChargerItems()
        {
            item = new List<ChargerItem>();
        }
    }


    public class ChargerModel
    {
        public string statNm { get; set; }
        public string statId { get; set; }
        public string addr { get; set; }
        public string location { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public double distance { get; set; }
        public string avail { get; set; }

        public string kind { get; set; }
        public string kindDetail { get; set; }
        public string kindNm { get; set; }
        public string kindDetailNm { get; set; }


        public List<ChargerItem> chgr { get; set; }
        public List<ChargerStatusItem> status { get; set; }

        public ChargerModel()
        {
            chgr = new List<ChargerItem>();
            status = new List<ChargerStatusItem>();
        }

    }



    public class ChargerItem : ChargerModel
    {
        

        public string chgerId { get; set; }
        public string chgerType { get; set; }

        public string useTime { get; set; }
        public string busiId { get; set; }
        public string bnm { get; set; }
        public string busiNm { get; set; }
        public string busiCall { get; set; }
        public string stat { get; set; }
        public string statUpdDt { get; set; }
        public string lastTsdt { get; set; }
        public string lastTedt { get; set; }
        public string nowTsdt { get; set; }
        public string powerType { get; set; }
        public string output { get; set; }
        public string method { get; set; }
        public string zcode { get; set; }
        public string zscode { get; set; }
        public string parkingFree { get; set; }
        public string note { get; set; }
        public string limitYn { get; set; }
        public string limitDetail { get; set; }
        public string delYn { get; set; }
        public string delDetail { get; set; }
    }



    #endregion

    #region /////충전상태 조회 모델

    public class ChargerStatusResModel
    {
        public string resultCode { get; set; }
        public string resultMsg { get; set; }
        public int numOfRows { get; set; }
        public int pageNo { get; set; }
        public int totalCount { get; set; }

        public ChargerStatusItems items { get; set; }

        public ChargerStatusResModel()
        {
            items = new ChargerStatusItems();
        }
    }



    public class ChargerStatusItems
    {
        public List<ChargerStatusItem> item { get; set; }

        public ChargerStatusItems()
        {
            item = new List<ChargerStatusItem>();
        }

    }

    public class ChargerStatusItem
    {
        public string busiId { get; set; }
        public string statId { get; set; }
        public string chgerId { get; set; }
        public string stat { get; set; }

        public string statUpdDt { get; set; }
        public string lastTedt { get; set; }
        public string nowTsdt { get; set; }
    }

    #endregion


    public class  ChargerComment
    {
        public string user { get; set; }
        public string contentid { get; set; }
        public string comment { get; set; }
        public string ymd { get; set; }
        public bool me { get; set; }
    }
    
}
