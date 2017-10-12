using SFA.DAS.UI.Activities.Areas.ActivitiesList.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SFA.DAS.UI.Activities.Areas.ActivitiesList.Controllers
{
    public class DefaultController : Controller
    {
        // GET: ActivitiesList/Default
        public ActionResult Index()
        {
            var messages = new List<ActivityModel>();
            messages.Add(new ActivityModel("acc1", "activityType1","Description 1", "url",DateTime.Now.ToString("O")));
            messages.Add(new ActivityModel("acc2", "activityType1", "Description 2", "url", DateTime.Now.ToString("O")));
            return PartialView(messages);
        }
    }
}