using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nest;
using NuGet;

namespace SFA.DAS.UI.Activities.DataAccess.Repositories
{
    public interface IActivitiesUiRepository
    {
        IEnumerable<Activity> GetActivities(long accountId);

        IEnumerable<Activity> GetActivitiesGroupedByDayAndType(long accountId);

        IReadOnlyCollection<Hit<Activity>> GetAggregations(long accountId);

        ISearchResponse<Activity> GetAggregations2(long accountId);

        ISearchResponse<Activity> GetAggregations3(long accountId);
    }
}
