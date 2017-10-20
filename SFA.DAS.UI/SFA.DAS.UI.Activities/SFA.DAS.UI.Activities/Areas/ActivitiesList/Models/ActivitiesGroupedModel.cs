using System;
using NuGet;

namespace SFA.DAS.UI.Activities.Areas.ActivitiesList.Models
{
    public class ActivitiesGroupedModel
    {
        public ActivitiesGroupedModel(Activity.ActivityType activityType,  string description) : this(activityType,description,null)
        {

        }

        public ActivitiesGroupedModel(Activity.ActivityType activityType, string description, string byWhomText)
        {
            ActivityType = activityType;
            Description = description;
            ByWhomText = byWhomText;
        }

        public Activity.ActivityType ActivityType { get; }

        public string Description { get; }

        public string ByWhomText { get; }
    }
}