
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
                activityModels.Add(new ActivityModel(activity.DescriptionOne, activity.DescriptionTwo,activity.Url, CreateDayText(activity.PostedDateTime)));
            }
            var viewModel = new ActivitiesListModel(activityModels);
            return View(viewModel);
        }

        private string CreateDayText(DateTime datetime)
        {
            if (datetime.Date == DateTime.Now.Date)
                return "Today";
            if (datetime.Date == DateTime.Now.Subtract(new TimeSpan(1,0,0,0)))
                return "Yesterday";
            return datetime.ToString("d/mm/yyyy");
        }
    }
}