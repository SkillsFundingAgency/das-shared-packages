using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet
{
    public class FluentActivity
    {
        private Activity _activty;

        public FluentActivity OwnerId(string ownerId)
        {
            _activty.OwnerId = ownerId;
            return this;
        }

        public FluentActivity Description(string description)
        {
            _activty.Description = description;
            return this;
        }

        public FluentActivity Url(string url)
        {
            _activty.Url = url;
            return this;
        }

        public FluentActivity ActivityType(string activityType)
        {
            _activty.ActivityType = activityType;
            return this;
        }

        public FluentActivity PostedDateTime(string postedDateTime)
        {
            _activty.PostedDateTime = DateTime.Parse(postedDateTime, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
            return this;
        }

        public Activity Object()
        {
            return _activty;
        }

    }
}
