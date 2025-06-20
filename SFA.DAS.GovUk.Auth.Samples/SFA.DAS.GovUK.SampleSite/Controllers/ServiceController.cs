using System.Text.Json;
using System.Text.Json.Nodes;
using FluentValidation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.GovUK.Auth.Exceptions;
using SFA.DAS.GovUK.Auth.Models;
using SFA.DAS.GovUK.Auth.Services;
using SFA.DAS.GovUK.SampleSite.Extensions;
using SFA.DAS.GovUK.SampleSite.Models;


namespace SFA.DAS.GovUK.SampleSite.Controllers
{
    [AllowAnonymous]
    [Route("services", Name = "Service", Order = 0)]
    public class ServiceController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IStubAuthenticationService _stubAuthenticationService;
        private readonly IValidator<SignInStubViewModel> _signInStubViewModelValidator;

        public ServiceController(IConfiguration config, IStubAuthenticationService stubAuthenticationService, IValidator<SignInStubViewModel> signInStubViewModelValidator)
        {
            _config = config;
            _stubAuthenticationService = stubAuthenticationService;
            _signInStubViewModelValidator = signInStubViewModelValidator;
        }

        [HttpGet]
        [Route("sign-in-stub", Name = "SignIn-Stub")]
        [AllowAnonymous]
        public IActionResult SignInStub(string returnUrl)
        {
            return View("SignInStub", new SignInStubViewModel
            {
                Id = ModelState.IsValid ? _config["StubId"] : ModelState[nameof(SignInStubViewModel.Id)]?.AttemptedValue,
                Email = ModelState.IsValid ? _config["StubEmail"] : ModelState[nameof(SignInStubViewModel.Email)]?.AttemptedValue,
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [Route("sign-in-stub", Name = "SignIn-Stub")]
        [AllowAnonymous]
        public async Task<IActionResult> SignInStubPost(SignInStubViewModel model)
        {
            if(!await _signInStubViewModelValidator.ModelStateIsValid(model, ModelState))
                return RedirectToRoute("SignIn-Stub", new { model.ReturnUrl });

            try
            {
                GovUkUser? govUkUser = await _stubAuthenticationService.GetStubVerifyGovUkUser(model.UserFile);

                var claims = await _stubAuthenticationService.GetStubSignInClaims(new StubAuthUserDetails
                {
                    Id = model.Id,
                    Email = model.Email,
                    GovUkUser = govUkUser
                });

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claims,
                    new AuthenticationProperties());
            }
            catch (StubVerifyException ex)
            {
                ModelState.AddModelError(nameof(model.UserFile), ex.Message);
                return RedirectToRoute("SignIn-Stub", new { model.ReturnUrl });
            }

            return RedirectToRoute("SignedIn-stub", new { model.ReturnUrl });
        }

        [Authorize]
        [HttpGet]
        [Route("signed-in-stub", Name = "SignedIn-stub")]
        public IActionResult SignedInStub(string returnUrl)
        {
            return View(model: returnUrl);
        }
    }
}
