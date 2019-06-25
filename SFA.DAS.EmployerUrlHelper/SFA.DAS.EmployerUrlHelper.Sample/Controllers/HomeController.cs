using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.EmployerUrlHelper.Sample.Controllers
{
    [Route("")]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}