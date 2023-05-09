using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.GovUK.Auth.Models;
using SFA.DAS.GovUK.Auth.Services;
using SFA.DAS.GovUK.SampleSite.AppStart;

namespace SFA.DAS.GovUK.SampleSite.Controllers;

public class HomeController : Controller
{
    private readonly IStubAuthenticationService _stubAuthenticationService;

    public HomeController(IStubAuthenticationService stubAuthenticationService)
    {
        _stubAuthenticationService = stubAuthenticationService;
    }
    [HttpGet]
    public IActionResult Index()
    {
        return View();
    }
    
    [HttpGet]
    public IActionResult AccountDetails()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> AccountDetails(StubAuthUserDetails model)
    {
        var claims = await _stubAuthenticationService.GetStubSignInClaims(model);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claims,
            new AuthenticationProperties());
        
        return RedirectToAction("Authenticated");
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
            authenticationProperties, 
            new[] {
                CookieAuthenticationDefaults.AuthenticationScheme, 
                OpenIdConnectDefaults.AuthenticationScheme});
    }
    
}