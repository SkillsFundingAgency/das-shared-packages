using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.GovUK.Auth.Authentication;
using SFA.DAS.GovUK.Auth.Controllers.Routes;
using SFA.DAS.GovUK.Auth.Services;
using SFA.DAS.GovUK.SampleSite.Controllers.Routes;

namespace SFA.DAS.GovUK.SampleSite.Controllers
{

    [Route(HomeRoutes.Paths.Controller)]
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

        [HttpGet(HomeRoutes.Paths.Index, Name = HomeRoutes.Names.Index)]
        public IActionResult Index()
        {
            if (HttpContext?.User?.Identity?.IsAuthenticated ?? false)
            {
                return RedirectToRoute(HomeRoutes.Names.Home);
            }

            return View();
        }

        [HttpPost(HomeRoutes.Paths.Start, Name = HomeRoutes.Names.Start)]
        [ValidateAntiForgeryToken]
        public IActionResult Start(bool suspend = false)
        {
            HttpContext.Session.SetString("user:suspended", suspend ? "1" : "0");

            return RedirectToRoute(HomeRoutes.Names.Home);
        }

        [HttpGet(HomeRoutes.Paths.Home, Name = HomeRoutes.Names.Home)]
        [Authorize(Policy = nameof(PolicyNames.IsActiveAccount))]
        public IActionResult Home()
        {
            var returnUrl = _linkGenerator.GetPathByName(
                HttpContext,
                HomeRoutes.Names.VerifiedAccountDetails);

            return View(model: returnUrl ?? "/");
        }

        [HttpGet(HomeRoutes.Paths.AccountDetails, Name = HomeRoutes.Names.AccountDetails)]
        [Authorize(Policy = nameof(PolicyNames.IsActiveAccount))]
        public async Task<IActionResult> AccountDetails()
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var details = await _govUkAuthenticationService.GetAccountDetails(token);

            return View(details);
        }

        [HttpGet(HomeRoutes.Paths.ActiveStatus, Name = HomeRoutes.Names.ActiveStatus)]
        [Authorize(Policy = nameof(PolicyNames.IsAuthenticated))]
        public IActionResult ActiveStatus()
        {
            return View();
        }

        [HttpGet(HomeRoutes.Paths.VerifiedAccountDetails, Name = HomeRoutes.Names.VerifiedAccountDetails)]
        [Authorize(Policy = nameof(PolicyNames.IsVerified))]
        public async Task<IActionResult> VerifiedAccountDetails()
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var details = await _govUkAuthenticationService.GetAccountDetails(token);

            return View(details);
        }

        [HttpGet(HomeRoutes.Paths.ExplainVerify, Name = HomeRoutes.Names.ExplainVerify)]
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

        [HttpGet(HomeRoutes.Paths.SignOut, Name = HomeRoutes.Names.SignOut)]
        [AllowAnonymous]
        public async Task<IActionResult> SigningOut()
        {
            var idToken = await HttpContext.GetTokenAsync("id_token");

            var authenticationProperties = new AuthenticationProperties
            {
                RedirectUri = Url.RouteUrl(HomeRoutes.Names.SignedOut)
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

        [HttpGet(HomeRoutes.Paths.SignedOut, Name = HomeRoutes.Names.SignedOut)]
        [AllowAnonymous]
        public IActionResult UserSignedOut()
        {
            return View();
        }

        [HttpGet(HomeRoutes.Paths.Suspended, Name = HomeRoutes.Names.Suspended)]
        public IActionResult UserSuspended()
        {
            return View();
        }
    }
}