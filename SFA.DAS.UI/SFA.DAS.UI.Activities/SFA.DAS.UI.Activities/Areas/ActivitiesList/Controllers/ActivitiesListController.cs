using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SFA.DAS.UI.Activities.Areas.ActivitiesList.Mappers;
using SFA.DAS.UI.Activities.Areas.ActivitiesList.Models;
using SFA.DAS.UI.Activities.DataAccess.Repositories;
using SFA.DAS.UI.Activities.Domain.Configurations;

namespace SFA.DAS.UI.Activities.Areas.ActivitiesList.Controllers
{
    public class ActivitiesListController : Controller
    {
        private readonly IActivitiesUiRepository _repository;

        public ActivitiesListController()
        {
            _repository = new ActivitiesRepository(new ActivitiesConfiguration());
        }

        // GET: ActivitiesList/Default
        public ActionResult Index()
        {
            var activities = _repository.GetActivities(1234);

            var summarisedActivities = new ActivityMapper().SummariseCollections(activities);

            var activitiesView = new ActivitiesListModel(summarisedActivities.ToList());
            return PartialView(activitiesView);
        }
    }
}