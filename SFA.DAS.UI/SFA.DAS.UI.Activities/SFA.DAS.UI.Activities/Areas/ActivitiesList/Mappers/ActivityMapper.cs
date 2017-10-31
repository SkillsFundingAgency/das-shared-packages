using System;
using System.Collections.Generic;
using System.Linq;
using NuGet;
using SFA.DAS.UI.Activities.Areas.ActivitiesList.Models;

namespace SFA.DAS.UI.Activities.Areas.ActivitiesList.Mappers
{
    public class ActivityMapper
    {
        public IEnumerable<ActivitiesGroupedModel> SummariseCollections(IEnumerable<Activity> activities)
        {
            var activitiesGroupedByType = activities.GroupBy(a => a.TypeOfActivity).ToList();
            foreach (var grouping in activitiesGroupedByType)
            {
                SummariseACollection(grouping);
            }
            return activitiesGroupedByType.Select(SummariseACollection);
        }

        private ActivitiesGroupedModel SummariseACollection(IGrouping<string, Activity> activitiesOfSameType)
        {
            var firstOfType = activitiesOfSameType.First();
            var description = activitiesOfSameType.Count() > 1
                ? $"{activitiesOfSameType.Count()} {firstOfType.DescriptionPlural}"
                : $"1 {firstOfType.DescriptionSingular}";
            var byWhom = activitiesOfSameType.Count() > 1
                ? string.Empty
                : $"By {firstOfType.OwnerId}";

            return new ActivitiesGroupedModel(activitiesOfSameType.First().TypeOfActivity, description, byWhom, DateTime.Now);
        }

    }
}