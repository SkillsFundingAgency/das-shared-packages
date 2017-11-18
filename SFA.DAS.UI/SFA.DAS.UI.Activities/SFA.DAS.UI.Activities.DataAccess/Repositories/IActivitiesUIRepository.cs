using System;
using System.Collections.Generic;
using Nest;
using NuGet;

namespace SFA.DAS.UI.Activities.DataAccess.Repositories
{
    public interface IActivitiesUiRepository
    {
        IEnumerable<Activity> GetActivities(long accountId);

        ISearchResponse<Activity> GetAggregationsByType(long accountId);

        ISearchResponse<Activity> GetAggregationsByDay(long accountId);

    }
}
