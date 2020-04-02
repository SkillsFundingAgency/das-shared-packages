﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace SFA.DAS.MA.Shared.UI.TestSite.Framework.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>(), "oauth"));
            return View();
        }

        public ActionResult Cdn()
        {

            return View();
        }

    }
}