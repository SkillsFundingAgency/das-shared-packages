using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SFA.DAS.UI.Activities.Areas.ActivitiesList.Controllers;

namespace WebApplication1.Controllers
{
    public class ActivitiesController : Controller
    {
        // GET: Activities
        public ActionResult Index()
        {
            //SFA.DAS.UI.Activities.Areas.ActivitiesList.Controllers.DefaultController controller;
            return View();
        }
    }
}