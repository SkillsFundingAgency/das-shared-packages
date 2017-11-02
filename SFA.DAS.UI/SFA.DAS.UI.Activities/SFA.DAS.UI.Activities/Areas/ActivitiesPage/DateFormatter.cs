using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA.DAS.UI.Activities.Areas.ActivitiesPage
{
    public class DateFormatter
    {
        public static string CreateDayText(DateTime datetime)
        {
            if (datetime.Date == DateTime.Now.Date)
                return "Today";
            if (datetime.Date == DateTime.Now.Subtract(new TimeSpan(1, 0, 0, 0)).Date)
                return "Yesterday";
            return datetime.ToString("yyyy/MM/dd");
        }
    }
}