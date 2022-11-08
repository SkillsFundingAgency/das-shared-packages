using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.GovUk.SampleSite.Legacy.Controllers
{

    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Authenticated()
        {
            return View();
        }

        [Authorize]
        [HttpGet]
        [Route("sign-out")]
        public async Task<IActionResult> SigningOut()
        {
            var idToken = await HttpContext.GetTokenAsync("id_token");

            var authenticationProperties = new AuthenticationProperties();
            authenticationProperties.Parameters.Clear();
            authenticationProperties.Parameters.Add("id_token", idToken);
            return SignOut(
                authenticationProperties, CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }

    }
}