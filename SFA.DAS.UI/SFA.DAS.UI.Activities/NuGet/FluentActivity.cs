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
        private Activity _activity=new Activity();

        public FluentActivity OwnerId(string ownerId)
        {
            _activity.OwnerId = ownerId;
            return this;
        }

        public FluentActivity DescriptionSingular(string description)
        {
            _activity.DescriptionSingular = description;
            return this;
        }

        public FluentActivity DescriptionPlural(string description)
        {
            _activity.DescriptionPlural = description;
            return this;
        }

        public FluentActivity DescriptionFull(string description)
        {
            _activity.DescriptionFull = description;
            return this;
        }

        public FluentActivity Url(string url)
        {
            _activity.Url = url;
            return this;
        }

        public FluentActivity ActivityType(Activity.ActivityType activityType)
        {
            _activity.Type = activityType;
            return this;
        }

        public FluentActivity PostedDateTime(DateTime postedDateTime)
        {
            _activity.PostedDateTime = postedDateTime;
            return this;
        }

        public FluentActivity AddAssociatedThing(string thing)
        {
            _activity.AssociatedData.Add(thing);
            return this;
        }

        public FluentActivity AddAssociatedThings(IEnumerable<string> things)
        {
            foreach (var thing in things)
            {
                _activity.AssociatedData.Add(thing);
            }        
            return this;
        }

        public Activity Object()
        {
            return _activity;
        }

    }
}
