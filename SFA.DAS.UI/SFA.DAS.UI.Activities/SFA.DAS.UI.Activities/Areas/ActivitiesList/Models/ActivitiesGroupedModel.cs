using System;
using NuGet;

namespace SFA.DAS.UI.Activities.Areas.ActivitiesList.Models
{
    public class ActivitiesGroupedModel
    {
        public ActivitiesGroupedModel(string activityType,  string description, DateTime when) : this(activityType,description,null, when)
        {

        }

        public ActivitiesGroupedModel(string activityType, string description, string byWhomText, DateTime when)
        {
            ActivityType = activityType;
            Description = description;
            ByWhomText = byWhomText;
            When = when;
        }

        public DateTime When { get; }

        public string ActivityType { get; }

        public string Description { get; }

        public string ByWhomText { get; }
    }
}