using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.GovUK.Auth.Authentication;
using SFA.DAS.GovUK.Auth.Controllers.Routes;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.GovUK.SampleSite.Controllers
{

    [Route(Routes.HomeRoutes.Paths.Controller)]
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IGovUkAuthenticationService _govUkAuthenticationService;
        private readonly LinkGenerator _linkGenerator;

        public HomeController(
            IConfiguration configuration,
            IGovUkAuthenticationService govUkAuthenticationService,
            LinkGenerator linkGenerator)
        {
            _configuration = configuration;
            _govUkAuthenticationService = govUkAuthenticationService;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        public IActionResult Index()
        {
            if (HttpContext?.User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToRoute(Routes.HomeRoutes.Names.Home);
            }

            return View();
        }

        [HttpPost(Routes.HomeRoutes.Paths.Start, Name = Routes.HomeRoutes.Names.Start)]
        [ValidateAntiForgeryToken]
        public IActionResult Start(bool suspend = false)
        {
            HttpContext.Session.SetString("user:suspended", suspend ? "1" : "0");

            return RedirectToRoute(Routes.HomeRoutes.Names.Home);
        }

        [HttpGet(Routes.HomeRoutes.Paths.Home, Name = Routes.HomeRoutes.Names.Home)]
        [Authorize(Policy = nameof(PolicyNames.IsAuthenticated))]
        public IActionResult Home()
        {
            var returnUrl = _linkGenerator.GetPathByName(
                HttpContext,
                Routes.HomeRoutes.Names.VerifiedAccountDetails);

            return View(model: returnUrl ?? "/");
        }

        [HttpGet(Routes.HomeRoutes.Paths.AccountDetails, Name = Routes.HomeRoutes.Names.AccountDetails)]
        [Authorize(Policy = nameof(PolicyNames.IsAuthenticated))]
        public async Task<IActionResult> AccountDetails()
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var details = await _govUkAuthenticationService.GetAccountDetails(token);

            return Content(JsonSerializer.Serialize(details), "application/json");
        }

        [HttpGet(Routes.HomeRoutes.Paths.IsActive, Name = Routes.HomeRoutes.Names.IsActive)]
        [Authorize(Policy = nameof(PolicyNames.IsActiveAccount))]
        public IActionResult IsActive()
        {
            return View();
        }

        [HttpGet(Routes.HomeRoutes.Paths.VerifiedAccountDetails, Name = Routes.HomeRoutes.Names.VerifiedAccountDetails)]
        [Authorize(Policy = nameof(PolicyNames.IsVerified))]
        public IActionResult VerifiedAccountDetails()
        {
            return RedirectToRoute(Routes.HomeRoutes.Names.AccountDetails);
        }

        [HttpGet(Routes.HomeRoutes.Paths.ExplainVerify, Name = Routes.HomeRoutes.Names.ExplainVerify)]
        [Authorize(Policy = nameof(PolicyNames.IsActiveAccount))]
        public IActionResult ExplainVerify(string returnUrl = "/")
        {
            if (!Url.IsLocalUrl(returnUrl))
            {
                returnUrl = "/";
            }

            return View(model: returnUrl);
        }

        [HttpPost(Routes.HomeRoutes.Paths.ExplainVerify)]
        [Authorize(Policy = nameof(PolicyNames.IsActiveAccount))]
        [ValidateAntiForgeryToken]
        public IActionResult ExplainVerifyContinue(string returnUrl = "/")
        {
            if (!Url.IsLocalUrl(returnUrl))
            {
                returnUrl = "/";
            }

            return Redirect($"{ServiceRoutes.Paths.VerifyIdentity.ServiceControllerPath()}?returnUrl={Uri.EscapeDataString(returnUrl)}");
        }

        [HttpGet(Routes.HomeRoutes.Paths.SignOut, Name = Routes.HomeRoutes.Names.SignOut)]
        [AllowAnonymous]
        public async Task<IActionResult> SigningOut()
        {
            var idToken = await HttpContext.GetTokenAsync("id_token");

            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.RouteUrl(Routes.HomeRoutes.Names.SignedOut)
            };

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

        [HttpGet(Routes.HomeRoutes.Paths.SignedOut, Name = Routes.HomeRoutes.Names.SignedOut)]
        [AllowAnonymous]
        public IActionResult UserSignedOut()
        {
            return View();
        }

        [HttpGet(Routes.HomeRoutes.Paths.Suspended, Name = Routes.HomeRoutes.Names.Suspended)]
        public IActionResult UserSuspended()
        {
            return View();
        }
    }
}