using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapView.Common.Util
{
    public static class Helper
    {
        public static string GetTitle(string controllerName)
        {
			var title = "맵뷰";
			if (controllerName.ToLower() == "camp")
			{
				title = "캠핑장";
			}
			else if (controllerName.ToLower() == "charger")
			{
				title = "EV충전소";
			}
			else if (controllerName.ToLower() == "festival")
			{
				title = "지역축제";
			}
			else if (controllerName.ToLower() == "webtoon")
            {
				title = "테스트";
			}
			else
			{
				title = "맵뷰";
			}

			return title;
		}


		public static string GetWeek(string week)
		{
			var title = "일요일";
			if (week.ToLower() == "1")
			{
				title = "월요일";
			}
			else if (week.ToLower() == "2")
			{
				title = "화요일";
			}
			else if (week.ToLower() == "3")
			{
				title = "수요일";
			}
			else if (week.ToLower() == "4")
			{
				title = "목요일";
			}
			else if (week.ToLower() == "5")
			{
				title = "금요일";
			}
			else if (week.ToLower() == "6")
			{
				title = "토요일";
			}

			return title;
		}

	}
}
