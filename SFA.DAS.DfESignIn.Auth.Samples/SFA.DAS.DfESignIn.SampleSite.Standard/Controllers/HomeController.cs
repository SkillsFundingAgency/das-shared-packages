using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.DfESignIn.SampleSite.Standard.Controllers
{
    [Route("")]
    [Route("[controller]")]
    public class HomeController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpGet("authenticated")]
        public IActionResult Authenticated()
        {
            return View();
        }

        [Authorize]
        [HttpGet("sign-out")]
        public async Task<IActionResult> SigningOut()
        {
            var idToken = await HttpContext.GetTokenAsync("id_token");

            var authenticationProperties = new AuthenticationProperties();
            authenticationProperties.Parameters.Clear();
            authenticationProperties.Parameters.Add("id_token", idToken);
            return SignOut(
                authenticationProperties,
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}