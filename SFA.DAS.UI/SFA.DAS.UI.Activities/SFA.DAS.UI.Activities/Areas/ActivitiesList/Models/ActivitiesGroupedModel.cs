using System;
using NuGet;

namespace SFA.DAS.UI.Activities.Areas.ActivitiesList.Models
{
    public class ActivitiesGroupedModel
    {
        public ActivitiesGroupedModel(string activityType,  string description) : this(activityType,description,null)
        {

        }

        public ActivitiesGroupedModel(string activityType, string description, string byWhomText)
        {
            ActivityType = activityType;
            Description = description;
            ByWhomText = byWhomText;
        }

        public string ActivityType { get; }

        public string Description { get; }

        public string ByWhomText { get; }
    }
}