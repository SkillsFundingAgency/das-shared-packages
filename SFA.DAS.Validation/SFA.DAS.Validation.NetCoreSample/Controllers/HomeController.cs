using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.Validation.NetCoreSample.Controllers
{
    [Route("")]
    public class HomeController : ControllerBase
    {
        public IActionResult Index()
        {
            return RedirectToAction("Create", "Accounts");
        }
    }
}