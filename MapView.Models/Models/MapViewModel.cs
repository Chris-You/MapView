using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using System.Text.Json.Serialization;
using MapView.Common.Models.Festival;

namespace MapView.Common.Models
{
    public enum ServiceGubun
    {
        camp,
        charger,
        festival
    }

    public class Favor
    {
        public ObjectId Id { get; set; }
        [JsonIgnore]
        public string user { get; set; }
        public ServiceGubun service { get; set; }
        public string contentId { get; set; }
        public string contentNm { get; set; }
        public string thumImage { get; set; }
        public string addr { get; set; }
        public string tel { get; set; }
        public string zscode { get; set; }
        public DateTime date { get; set; }
        public int totalCnt { get; set; }
        public int availCnt { get; set; }

        //public string cat1 { get; set; }
        //public string areacode { get; set; }


        public CampItem camp { get; set; }
        public FestivalItem festival { get; set; }

    }
}
