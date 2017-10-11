using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NuGetProject;

namespace SFA.DAS.Activities.Areas.ActivitiesList.Controllers
{
    public class DefaultController : Controller
    {
        [ChildActionOnly]
        public ActionResult Index()
        {
            var messages = new List<CreateActivityMessage>();

            return PartialView(messages);
        }
    }
}