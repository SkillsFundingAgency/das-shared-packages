
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using SFA.DAS.UI.Activities.Areas.ActivitiesPage.Models;
using SFA.DAS.UI.Activities.DataAccess.Repositories;
using SFA.DAS.UI.Activities.Domain.Configurations;

namespace SFA.DAS.UI.Activities.Areas.ActivitiesPage.Controllers
{
    public class DefaultController : Controller
    {
        private readonly IActivitiesUiRepository _repository;


        public DefaultController(IActivitiesUiRepository repository)
        {
            _repository = repository;
        }

        public DefaultController()
        {
            _repository = new ActivitiesRepository(new ActivitiesConfiguration());
        }

        // GET: ActivitiesPage/Default
        public ActionResult Index()
        {
            var activities = _repository.GetActivities("OwnerId");

            var activityModels = new List<ActivityModel>();
            foreach (var activity  in activities)
            {
                activityModels.Add(new ActivityModel(activity.DescriptionOne, activity.DescriptionTwo,activity.Url, DateFormatter.CreateDayText(activity.PostedDateTime)));
            }
            var viewModel = new ActivitiesListModel(activityModels);
            return View(viewModel);
        }


    }
}