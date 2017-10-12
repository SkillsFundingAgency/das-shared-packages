using System.Collections.Generic;
using System.Web.Mvc;
using NuGetProject;
using SFA.DAS.Activities.UI.Areas.ActivitiesList.Models;

namespace SFA.DAS.Activities.UI.Areas.ActivitiesList.Controllers
{
    public class DefaultController : Controller
    {
        [ChildActionOnly]
        public ActionResult Index()
        {
            var messages = new List<Activity>();

            return PartialView(messages);
        }
    }
}