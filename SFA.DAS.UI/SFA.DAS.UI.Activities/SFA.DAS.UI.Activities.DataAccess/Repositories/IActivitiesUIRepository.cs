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

    }
}
