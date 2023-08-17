using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Provider.Shared.UI.Attributes;
using SFA.DAS.Provider.Shared.UI;

namespace SFA.DAS.DfESignIn.SampleSite.Controllers;

[SetNavigationSection(NavigationSection.YourCohorts)]
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
    [Route("signed-out")]
    public async Task<IActionResult> SigningOut()
    {
        var idToken = await HttpContext.GetTokenAsync("id_token");

        var authenticationProperties = new AuthenticationProperties();
        authenticationProperties.Parameters.Clear();
        authenticationProperties.Parameters.Add("id_token",idToken);
        return SignOut(
            authenticationProperties,
            new[]
            {
                CookieAuthenticationDefaults.AuthenticationScheme,
                OpenIdConnectDefaults.AuthenticationScheme
            }
        );
    }

    [HttpGet]
    [Route("dashboard")]
    public IActionResult RedirectToDashboard()
    {
        return RedirectPermanent("https://test-pas.apprenticeships.education.gov.uk/");
    }
}