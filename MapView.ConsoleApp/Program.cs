using System;
using StackExchange.Redis;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using HtmlAgilityPack;
using MapView.Common.Models.Webtoon;

namespace CampingView.ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            int titleId = 758037;
            int no = 1;

            var html = "https://comic.naver.com/webtoon/detail?titleId=" + +titleId + "&no=" + no;

            HtmlWeb web = new HtmlWeb();

            var htmlDoc = web.Load(html);

            var node = htmlDoc.DocumentNode.SelectSingleNode("//head/title");

            var body = htmlDoc.DocumentNode.SelectSingleNode("//body/div");

            var container = body.SelectNodes("//div").Where(w => w.Id == "container").First();

            //var content = container.SelectSingleNode("//div");

            var doc = new HtmlDocument();
            doc.LoadHtml(container.InnerHtml);

            var doc2 = new HtmlDocument();
            var node2 = doc.DocumentNode.SelectNodes("//div/div").Where( w => w.Id == "sectionContWide").First();
            doc2.LoadHtml(node2.InnerHtml);

            


            // 썸네일 영역
            var thumb = doc2.DocumentNode.SelectNodes("//div/div")[0];
            var img = thumb.SelectSingleNode("//a/img");
            var src = img.Attributes["src"].Value;

            var detail = doc2.DocumentNode.SelectNodes("//div/div")[1];
            var title = thumb.SelectNodes("//h2/span")[0].InnerText;
            var artist = thumb.SelectNodes("//h2/span")[1].InnerText;
            var txt = thumb.SelectNodes("//p").Where( w=> w.Attributes["class"].Value == "txt").First().InnerText;
            var genre = thumb.SelectNodes("//p/span")[0].InnerText;
            var age = thumb.SelectNodes("//p/span")[1].InnerText;
            //var age2 = age.SelectSingleNode("//span")[0];


            // 회차정보
            var listNo = doc2.DocumentNode.SelectNodes("//div/div")[1];
            var noName = listNo.SelectSingleNode("//div/h3").InnerText;
            var star = listNo.SelectNodes("//div/dl/dd/div/span").Where(w => w.Id == "topPointTotalNumber").First().InnerText;
            var regDt = listNo.SelectNodes("//div").Where(w => w.Attributes["class"].Value == "vote_lst").First()
                              .SelectNodes("dl").Where(w => w.Attributes["class"].Value == "rt").First()
                              .SelectNodes("dd").First().InnerText;

            // 웹툰 컨텐츠

            var view = doc.DocumentNode.SelectNodes("//div").Where(w => w.Id == "comic_view_area").First();

            var doc3 = new HtmlDocument();
            doc3.LoadHtml(view.InnerHtml);
            var content = doc3.DocumentNode.SelectNodes("//div")[0].OuterHtml;
            


            //var content = view.SelectSingleNode("//div").InnerHtml;


            // 작가의말
            var doc4 = new HtmlDocument();
            var node3 = node2.NextSibling.NextSibling;
            doc4.LoadHtml(node3.InnerHtml);
            var artistNm = doc4.DocumentNode.SelectNodes("//div/div/h4/em/strong").First().InnerText;
            var comment = doc4.DocumentNode.SelectNodes("//div/div/p").First().InnerHtml;


            var webtoon = new Contents();
            webtoon.titleId = titleId;
            webtoon.titleName = title;
            webtoon.titleImg = src;
            webtoon.gubun = "webtoon";
            webtoon.artist = artist.Replace("\t","");
            webtoon.thumImg = src;
            webtoon.desc = txt;
            webtoon.genre = genre;
            webtoon.age = age;
            webtoon.status = "Y";


            var item = new ContentsList();
            item.titleId = titleId;
            item.no = no;
            item.name = noName;
            item.content = content.Replace("\t", "").Replace("\n", "");
            item.comment = comment;
            item.status = "Y";
            item.regDate = DateTime.Now;


            Console.WriteLine("");

       }
    }
}
