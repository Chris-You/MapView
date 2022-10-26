using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;

namespace MapView.Common.Models.Webtoon
{
    /*
    public class Artist
    {
        public int id { get; set; }
        public int name { get; set; }
    }
    */

    public class Contents
    {
        public ObjectId Id { get; set; }
        public int titleId { get; set; }
        public string titleName { get; set; }
        public string titleImg { get; set; }
        public string gubun { get; set; }  /* webtoon, novel... */
        public string artist { get; set; }
        public string desc { get; set; }
        public string thumImg { get; set; }
        public string genre { get; set; }
        public string status { get; set; }
        public string age { get; set; }
        public string week { get; set; }  /* 0(일) ~ 6(토)   */
        public string star { get; set; }
        public string lastUpdDate { get; set; }

        public List<ContentsList> list { get; set; }

        public Contents() 
        {
            list = new List<ContentsList>();
        }
    }

    public class Favorites
    {
        public int titleId { get; set; }
        public string userId { get; set; }
        public DateTime regDate { get; set; }
    }


    public class ContentsList
    {
        public ObjectId Id { get; set; }
        public int titleId { get; set; }
        public int no { get; set; }
        public string name { get; set; }
        public string content { get; set; }
        public string comment { get; set; }
        public string status { get; set; }
        public float star { get; set; }
        public DateTime regDate { get; set; }
    }



    public class StarRate
    {
        public int titleId { get; set; }
        public int no { get; set; }
        public string userId { get; set; }
        public float star { get; set; }
        public DateTime regDate { get; set; }
    }
}
