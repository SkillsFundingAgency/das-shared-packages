using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Validation.NetCoreSample.RouteValues.Accounts;
using SFA.DAS.Validation.NetCoreSample.ViewModels.Accounts;

namespace SFA.DAS.Validation.NetCoreSample.Controllers
{
    [Route("accounts")]
    public class AccountsController : Controller
    {
        [Route("create")]
        public ActionResult Create(CreateAccountRouteValues routeValues)
        {
            return View(new CreateAccountViewModel
            {
                Username = routeValues.Username
            });
        }
        
        [HttpPost]
        [Route("create")]
        public IActionResult Create(CreateAccountViewModel model)
        {
            return RedirectToAction("Created");
        }
        
        [Route("created")]
        public ActionResult Created()
        {
            return View();
        }
    }
}