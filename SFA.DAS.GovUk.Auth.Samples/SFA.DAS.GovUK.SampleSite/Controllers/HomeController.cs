using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.GovUK.SampleSite.AppStart;

namespace SFA.DAS.GovUK.SampleSite.Controllers;

public class HomeController : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }

    [Authorize(Policy = nameof(PolicyNames.IsAuthenticated))]
    [HttpGet]
    public IActionResult Authenticated()
    {
        return View();
    }
    [Authorize(Policy = nameof(PolicyNames.IsActiveAccount))]
    [HttpGet]
    public IActionResult IsActive()
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
        authenticationProperties.Parameters.Add("id_token",idToken);
        return SignOut(
            authenticationProperties, CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme);
    }
    
}