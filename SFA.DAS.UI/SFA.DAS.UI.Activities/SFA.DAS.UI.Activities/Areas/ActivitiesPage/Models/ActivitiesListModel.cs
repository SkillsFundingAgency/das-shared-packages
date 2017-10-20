using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SFA.DAS.UI.Activities.Areas.ActivitiesPage.Models
{
    public class ActivitiesListModel
    {
        public ActivitiesListModel(List<ActivityModel> activities)
        {
            Activities = activities;
        }

        public List<ActivityModel> Activities { get; }
    }
}