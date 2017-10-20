using System;
using System.Collections.Generic;


namespace SFA.DAS.UI.Activities.Areas.ActivitiesList.Models
{
    public class ActivitiesListModel
    {
        public ActivitiesListModel(List<ActivitiesGroupedModel> activities)
        {
            Activities = activities;
        }

        public List<ActivitiesGroupedModel> Activities { get; }
    }
}