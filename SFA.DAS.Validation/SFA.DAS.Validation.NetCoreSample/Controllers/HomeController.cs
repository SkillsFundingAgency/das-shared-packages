using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Validation.NetCoreSample.RouteValues.Accounts;

namespace SFA.DAS.Validation.NetCoreSample.Controllers
{
    [Route("")]
    public class HomeController : ControllerBase
    {
        public IActionResult Index()
        {
            return RedirectToAction("Create", "Accounts", new CreateAccountRouteValues { Username = "Foobar" });
        }
    }
}