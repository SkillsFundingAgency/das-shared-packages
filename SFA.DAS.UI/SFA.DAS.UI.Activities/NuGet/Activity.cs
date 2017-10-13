using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet
{
    public class Activity
    {
        public Activity()
        {
            
        }

        //public Activity(string ownerId, string activityType, string description, string url, string postedDate)
        //{
        //    OwnerId = ownerId;
        //    ActivityType = activityType;
        //    Description = description;
        //    Url = url;
        //    PostedDateTime = DateTime.Parse(postedDate, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        //}

        public string OwnerId { get; internal set; }

        public string ActivityType { get; internal set; }

        public string Description { get; internal set; }

        public string Url { get; internal set; }

        public DateTime PostedDateTime { get; internal set; }
    }
}
