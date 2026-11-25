using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using SFA.DAS.DfESignIn.Auth.Configuration;
using SFA.DAS.DfESignIn.Auth.Extensions;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using SFA.DAS.DfESignIn.Auth.Models.Stub;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.DfESignIn.Auth.Controllers
{
    [AllowAnonymous]
    [Route("stub")]
    public class DfESignInStubController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly DfESignInStubConfiguration _stubConfiguration;
        private readonly IDfESignInStubService _stubService;
        private readonly DfEOidcConfiguration _oidcConfiguration;

        public DfESignInStubController(
            IConfiguration configuration,
            IOptions<DfESignInStubConfiguration> stubConfiguration,
            IDfESignInStubService stubService,
            IOptions<DfEOidcConfiguration> oidcConfiguration)
        {
            _configuration = configuration;
            _stubConfiguration = stubConfiguration.Value;
            _stubService = stubService;
            _oidcConfiguration = oidcConfiguration.Value;
        }

        [HttpGet("sign-in-stub")]
        public IActionResult SignInStub(string returnUrl = "/")
        {
            if (!IsStubAuthenticationEnabled())
            {
                return NotFound();
            }

            return View("SignInStub", new SignInStubViewModel
            {
                UserId = _stubConfiguration.DefaultUserId,
                ReturnUrl = returnUrl,
                Users = _stubConfiguration.Users
            });
        }

        [HttpPost("sign-in")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SignIn(
            SelectOrganisationStubViewModel model)
        {
            if (!IsStubAuthenticationEnabled() ||
                string.IsNullOrWhiteSpace(model.UserId))
            {
                return NotFound();
            }

            var user = _stubConfiguration.Users.FirstOrDefault(
                configuredUser => configuredUser.Id == model.UserId);

            if (user == null)
            {
                return NotFound();
            }

            if (!model.OrganisationId.HasValue)
            {
                ModelState.AddModelError(
                    nameof(model.OrganisationId),
                    "Select an organisation.");
            }
            else if (!user.Organisations.Any(
                         organisation => organisation.Id == model.OrganisationId.Value))
            {
                ModelState.AddModelError(
                    nameof(model.OrganisationId),
                    "Select an organisation that belongs to this user.");
            }

            if (!ModelState.IsValid)
            {
                model.Email = user.Email;
                model.Organisations = user.Organisations;

                return View("SelectOrganisation", model);
            }

            var principal = _stubService.GetStubSignInClaims(
                user.Id,
                model.OrganisationId.Value);

            if (principal == null)
            {
                return NotFound();
            }

            var returnUrl = Url.IsLocalUrl(model.ReturnUrl)
                ? model.ReturnUrl
                : Url.Content("~/");

            var properties = new AuthenticationProperties
            {
                RedirectUri = returnUrl
            };

            var customClaimsContext = new TokenValidatedContext(
                HttpContext,
                new AuthenticationScheme(
                    OpenIdConnectDefaults.AuthenticationScheme,
                    "DfE Sign-in stub",
                    typeof(OpenIdConnectHandler)),
                new OpenIdConnectOptions
                {
                    ClientId = _oidcConfiguration.ClientId
                },
                principal,
                properties);

            await customClaimsContext.PopulateCustomClaimsAsync();

            if (customClaimsContext.Result?.Handled == true)
            {
                return new EmptyResult();
            }

            if (customClaimsContext.Result?.Failure is not null)
            {
                return StatusCode(StatusCodes.Status403Forbidden);
            }

            if (customClaimsContext.Result?.Skipped == true)
            {
                throw new InvalidOperationException(
                    "The custom claims handler stopped stub sign-in.");
            }

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                customClaimsContext.Principal,
                customClaimsContext.Properties);

            return LocalRedirect(returnUrl);
        }

        [HttpGet("select-organisation")]
        public IActionResult SelectOrganisation(
            string userId,
            string returnUrl = "/")
        {
            if (!IsStubAuthenticationEnabled() ||
                string.IsNullOrWhiteSpace(userId))
            {
                return NotFound();
            }

            var user = _stubConfiguration.Users.FirstOrDefault(
                configuredUser => configuredUser.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            return View("SelectOrganisation", new SelectOrganisationStubViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                ReturnUrl = returnUrl,
                Organisations = user.Organisations
            });
        }

        private bool IsStubAuthenticationEnabled()
        {
            bool.TryParse(_configuration["StubAuth"], out var stubAuth);
            return stubAuth && !string.Equals(_configuration["ResourceEnvironmentName"], "PRD",  StringComparison.OrdinalIgnoreCase);
        }
    }
}