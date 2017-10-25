using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NuGet;

namespace SFA.DAS.UI.Activities.Areas.ActivitiesPage.Models
{
    public class ActivityModel
    {
        public ActivityModel(string type, string byWhomText, string description, string url)
        {
            Type = type;
            ByWhomText = byWhomText;
            Description = description;
            Url = url;
        }

        public string Type { get;  }

        public string ByWhomText { get; }

        public string Description { get; }

        public string Url { get; }
    }
}