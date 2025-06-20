using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.GovUK.Auth.Authentication;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.GovUK.SampleSite.Controllers;

public class HomeController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly IGovUkAuthenticationService _govUkAuthenticationService;

    public HomeController(IConfiguration configuration, IGovUkAuthenticationService govUkAuthenticationService)
    {
        _configuration = configuration;
        _govUkAuthenticationService = govUkAuthenticationService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        if (HttpContext?.User?.Identity?.IsAuthenticated ?? false)
            return RedirectToAction("Home");
        
        return View();
    }

    [HttpGet]
    [Authorize(Policy = nameof(PolicyNames.IsAuthenticated))]
    public IActionResult Home()
    {
        return View();
    }

    [Authorize(Policy = nameof(PolicyNames.IsAuthenticated))]
    [HttpGet]
    public async Task<IActionResult> AccountDetails()
    {
        var token = await HttpContext.GetTokenAsync("access_token");
        var details = await _govUkAuthenticationService.GetAccountDetails(token);

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
    public IActionResult VerifiedAccountDetails()
    {
        return RedirectToAction(nameof(AccountDetails));
    }

    [Authorize(Policy = nameof(PolicyNames.IsActiveAccount))]
    public IActionResult ExplainVerify()
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
        authenticationProperties.Parameters.Add("id_token", idToken);

        var authenticationSchemes = new[] { CookieAuthenticationDefaults.AuthenticationScheme };
        if (!bool.TryParse(_configuration["StubAuth"], out var stubAuth) || !stubAuth)
        {
            authenticationSchemes = authenticationSchemes
                .Append(OpenIdConnectDefaults.AuthenticationScheme)
                .ToArray();
        }

        return SignOut(
            authenticationProperties,
            authenticationSchemes);
    }

    [HttpGet]
    [Route("signed-out", Name = "SignedOut")]
    public IActionResult SignedOut()
    {
        return View();
    }
}