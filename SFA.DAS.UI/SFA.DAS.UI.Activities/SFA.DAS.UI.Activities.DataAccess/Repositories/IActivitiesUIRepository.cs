using System;
using System.Collections.Generic;
using NuGet;

namespace SFA.DAS.UI.Activities.DataAccess.Repositories
{
    public interface IActivitiesUiRepository
    {
        IEnumerable<Activity> GetActivities(long accountId);

    }
}
