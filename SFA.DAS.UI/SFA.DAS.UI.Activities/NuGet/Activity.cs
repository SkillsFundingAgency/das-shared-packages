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
        public Activity(string accountId, string activityType, string description, string url, string postedDate)
        {
            AccountId = accountId;
            ActivityType = activityType;
            Description = description;
            Url = url;
            PostedDateTime = DateTime.Parse(postedDate, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        }

        public string AccountId { get; }

        public string ActivityType { get; }

        public string Description { get; }

        public string Url { get; }

        public DateTime PostedDateTime { get; }
    }
}
