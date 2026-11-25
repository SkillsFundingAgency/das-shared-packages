using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.Provider.Shared.UI;
using SFA.DAS.Provider.Shared.UI.Attributes;

namespace SFA.DAS.DfESignIn.SampleSite.Controllers;

[SetNavigationSection(NavigationSection.YourCohorts)]
public class HomeController : Controller
{
    private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;

    public HomeController(
        IAuthenticationSchemeProvider authenticationSchemeProvider)
    {
        _authenticationSchemeProvider = authenticationSchemeProvider;
    }

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
    [Route("signed-out", Name = "provider-signout")]
    public async Task<IActionResult> SigningOut()
    {
        var signOutScheme = await _authenticationSchemeProvider
            .GetDefaultSignOutSchemeAsync();

        if (signOutScheme == null)
        {
            throw new InvalidOperationException(
                "No default sign-out scheme has been configured.");
        }

        if (signOutScheme.Name == CookieAuthenticationDefaults.AuthenticationScheme)
        {
            return SignOut(
                CookieAuthenticationDefaults.AuthenticationScheme);
        }

        var idToken = await HttpContext.GetTokenAsync("id_token");

        var authenticationProperties = new AuthenticationProperties();
        authenticationProperties.Parameters.Add("id_token", idToken);

        return SignOut(
            authenticationProperties,
            CookieAuthenticationDefaults.AuthenticationScheme,
            signOutScheme.Name);
    }

    [HttpGet]
    [Route("dashboard")]
    public IActionResult RedirectToDashboard()
    {
        return RedirectPermanent("https://test-pas.apprenticeships.education.gov.uk/");
    }
}