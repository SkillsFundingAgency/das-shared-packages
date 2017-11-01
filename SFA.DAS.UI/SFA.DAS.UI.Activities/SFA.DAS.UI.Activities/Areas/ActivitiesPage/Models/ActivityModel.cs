using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NuGet;

namespace SFA.DAS.UI.Activities.Areas.ActivitiesPage.Models
{
    public class ActivityModel
    {
        public ActivityModel(string type, string byWhomText, string description, string url, string dayText)
        {
            Type = type;
            ByWhomText = byWhomText;
            Description = description;
            Url = url;
            DayText = dayText;
        }

        public string Type { get;  }

        public string ByWhomText { get; }

        public string Description { get; }

        public string Url { get; }

        public string DayText { get; }
    }
}