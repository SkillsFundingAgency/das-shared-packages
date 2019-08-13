using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Validation.NetCoreSample.ViewModels.Accounts;

namespace SFA.DAS.Validation.NetCoreSample.Controllers
{
    [Route("accounts")]
    public class AccountsController : Controller
    {
        [Route("create")]
        public ActionResult Create()
        {
            return View(new CreateAccountViewModel());
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