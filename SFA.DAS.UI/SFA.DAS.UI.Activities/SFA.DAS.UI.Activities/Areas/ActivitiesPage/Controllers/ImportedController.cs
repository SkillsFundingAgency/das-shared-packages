using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SFA.DAS.HashingService;
using SFA.DAS.UI.Activities.Areas.ActivitiesPage.Models;
using SFA.DAS.UI.Activities.DataAccess.Repositories;
using SFA.DAS.UI.Activities.Domain.Configurations;

namespace SFA.DAS.UI.Activities.Areas.ActivitiesPage.Controllers
{
    public class ImportedController : Controller
    {
        private readonly IActivitiesUiRepository _repository;
        private readonly IHashingService _hashingService;

        public ImportedController()
        {
            _repository = new ActivitiesRepository(new ActivitiesConfiguration());
        }

        public ImportedController(IActivitiesUiRepository repository, IHashingService hashingService)
        {
            _repository = repository;
        }



        // GET: ActivitiesPage/Default
        public ActionResult Index()
        {
            var activities = _repository.GetActivities(1234);

            var activityModels = new List<ActivityModel>();
            foreach (var activity in activities)
            {
                activityModels.Add(new ActivityModel(activity.DescriptionOne, activity.DescriptionTwo, activity.Url, DateFormatter.CreateDayText(activity.PostedDateTime)));
            }
            var viewModel = new ActivitiesListModel(activityModels);
            return View(viewModel);
        }
    }
}