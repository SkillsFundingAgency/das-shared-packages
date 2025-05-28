using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.GovUK.Auth.Authentication;
using SFA.DAS.GovUK.Auth.Models;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.GovUK.SampleSite.Controllers;

public class HomeController(IStubAuthenticationService stubAuthenticationService, IOidcService oidcService)
    : Controller
{

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
        var claims = await stubAuthenticationService.GetStubSignInClaims(model);

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
    
    [Authorize(Policy = nameof(PolicyNames.IsAuthenticated))]
    [HttpGet]
    public async Task<IActionResult> GetAccountDetails()
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        var details = await oidcService.GetAccountDetails(token);

        return Content(JsonSerializer.Serialize(details), "application/json");
    }
    
    [Authorize(Policy = nameof(PolicyNames.IsActiveAccount))]
    [HttpGet]
    public IActionResult IsActive()
    {
        return View();
    }

    [Authorize(Policy = nameof(PolicyNames.IsVerified))]
    [HttpGet]
    public IActionResult GetVerifiedAccountDetails()
    {
        return RedirectToAction(nameof(GetAccountDetails));
    }

    [Authorize(Policy = nameof(PolicyNames.IsAuthenticated))]
    [HttpGet]
    public IActionResult NotVerified()
    {
        return View();
    }

    [HttpGet]
    [Route("sign-out", Name = "SignOut")]
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