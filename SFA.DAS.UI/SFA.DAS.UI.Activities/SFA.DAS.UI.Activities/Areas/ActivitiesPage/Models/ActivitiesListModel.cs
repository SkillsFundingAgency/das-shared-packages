using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA.DAS.UI.Activities.Areas.ActivitiesPage.Models
{
    public class ActivitiesListModel
    {
        public ActivitiesListModel(IEnumerable<ActivityModel> activities)
        {
            Activities = activities.ToList();
        }

        public List<ActivityModel> Activities { get; }
    }
}