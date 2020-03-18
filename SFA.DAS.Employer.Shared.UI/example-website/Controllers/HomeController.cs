using System.Threading.Tasks;
using DfE.Example.Web.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SFA.DAS.Employer.Shared.UI;
using SFA.DAS.Employer.Shared.UI.Attributes;

namespace DfE.Example.Web.Controllers
{
    [SetNavigationSection(NavigationSection.AccountsHome)]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        [Route("{employerAccountId}")]
        public IActionResult Index(string employerAccountId)
        {
            return View();
        }

        [HttpGet, Route("logout", Name = RouteNames.Logout_Get)]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync("Cookies");

            await HttpContext.SignOutAsync("oidc");
        }
    }
}
