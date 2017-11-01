using System;
using System.Collections.Generic;

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

        public FluentActivity DescriptionOne(string description)
        {
            _activity.DescriptionOne = description;
            return this;
        }

        public FluentActivity DescriptionTwo(string description)
        {
            _activity.DescriptionTwo = description;
            return this;
        }

        public FluentActivity Url(string url)
        {
            _activity.Url = url;
            return this;
        }

        public FluentActivity ActivityType(string activityType)
        {
            _activity.TypeOfActivity = activityType.ToString();
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

        public FluentActivity HashedAccountId(string hashedAccountId)
        {
            _activity.HashedAccountId = hashedAccountId;
            return this;
        }

        public Activity Object()
        {
            return _activity;
        }

    }
}
