using System;
using System.Collections.Generic;
using System.Linq;
using NuGet;
using SFA.DAS.UI.Activities.Areas.ActivitiesList.Models;

namespace SFA.DAS.UI.Activities.Mappers
{
    public class ActivityMapper
    {
        public static IEnumerable<ActivityModel> Map(IEnumerable<Activity> activities)
        {
            return activities.Select(Map);
        }

        public static ActivityModel Map(Activity activity)
        {
            return new ActivityModel(activity.OwnerId,activity.Type, activity.Description,activity.Url, activity.PostedDateTime);
        }
    }
}