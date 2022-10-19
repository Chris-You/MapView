using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MapView.Util
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
			else
			{
				title = "맵뷰";
			}

			return title;
		}

    }
}
