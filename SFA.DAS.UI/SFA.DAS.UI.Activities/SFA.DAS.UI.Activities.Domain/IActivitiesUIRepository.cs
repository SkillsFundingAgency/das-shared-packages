using NuGet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.UI.Activities.Domain
{
    public interface IActivitiesUIRepository
    {
        Task<IEnumerable<Activity>> GetActivities(string ownerId);

        //Task<Activity> GetActivity(string ownerId, ActivityType type);

        Task<Activity> GetActivity(Activity message);
    }
}
