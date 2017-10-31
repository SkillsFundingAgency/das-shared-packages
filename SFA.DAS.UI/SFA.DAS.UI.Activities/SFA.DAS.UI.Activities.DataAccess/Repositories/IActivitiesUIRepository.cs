using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;
using NuGet;

namespace SFA.DAS.UI.Activities.DataAccess.Repositories
{
    public interface IActivitiesUiRepository
    {
        IEnumerable<Activity> GetActivities(string ownerId);

        IEnumerable<Activity> GetActivitiesGroupedByDayAndType(string accountId);

        IReadOnlyCollection<Hit<Activity>> GetAggregations(string accountId);

        ISearchResponse<Activity> GetAggregations2(string accountId);

        ISearchResponse<Activity> GetAggregations3(string accountId);
    }
}
