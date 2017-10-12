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
            var messages = new List<Activity>();
            return PartialView(messages);
        }
    }
}