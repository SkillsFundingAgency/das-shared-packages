using NuGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.UI.Activities.Domain
{
    public interface IActivitiesUiRepository
    {
        Task<IEnumerable<Activity>> GetActivities(string ownerId);
    }
}
