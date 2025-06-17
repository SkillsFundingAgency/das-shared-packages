using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.GovUK.Auth.Models;
using SFA.DAS.GovUK.Auth.Services;
using SFA.DAS.GovUK.SampleSite.Models;


namespace SFA.DAS.GovUK.SampleSite.Controllers
{
    [AllowAnonymous]
    [Route("services", Name = "Service", Order = 0)]
    public class ServiceController : Controller
    {
        private readonly IConfiguration _config;
        private readonly IStubAuthenticationService _stubAuthenticationService;

        public ServiceController(IConfiguration config, IStubAuthenticationService stubAuthenticationService)
        {
            _config = config;
            _stubAuthenticationService = stubAuthenticationService;
        }

#if DEBUG
        [HttpGet]
        [Route("sign-in-stub", Name = "SignIn-Stub")]
        [AllowAnonymous]
        public IActionResult SignInStub(string returnUrl)
        {
            return View("SignInStub", new SignInStubViewModel { Id = _config["StubId"], Email = _config["StubEmail"], ReturnUrl = returnUrl });
        }

        [HttpPost]
        [Route("sign-in-stub", Name = "SignIn-Stub")]
        [AllowAnonymous]
        public async Task<IActionResult> SignInStubPost(SignInStubViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Form data is invalid.");

            if (string.IsNullOrWhiteSpace(model.Id) || string.IsNullOrWhiteSpace(model.Email))
                return BadRequest("Id and Email are required.");

            GovUkUser? govUkUser = null;
            if (model.UserFile != null && model.UserFile.Length > 0)
            {
                try
                {
                    using var reader = new StreamReader(model.UserFile.OpenReadStream());
                    var json = await reader.ReadToEndAsync();

                    var rootNode = JsonNode.Parse(json)?.AsObject();
                    if (rootNode == null) return BadRequest("Invalid JSON structure.");

                    if (rootNode.TryGetPropertyValue("https://vocab.account.gov.uk/v1/coreIdentityJWT", out var unwrappedNode))
                    {
                        var coreJwt = unwrappedNode.Deserialize<GovUkCoreIdentityJwt>();
                        var jwtString = CoreIdentityJwtConverter.SerializeStubCoreIdentityJwt(coreJwt);

                        rootNode.Remove("https://vocab.account.gov.uk/v1/coreIdentityJWT");
                        rootNode["https://vocab.account.gov.uk/v1/coreIdentityJWT"] = jwtString;
                    }

                    var encryptedJson = rootNode.ToJsonString();
                    govUkUser = JsonSerializer.Deserialize<GovUkUser>(encryptedJson);
                    var t = govUkUser.CoreIdentityJwt.Vc.CredentialSubject.GetHistoricalNames();

                }
                catch
                {
                    return BadRequest("Invalid JSON file.");
                }
            }

            var claims = await _stubAuthenticationService.GetStubSignInClaims(new StubAuthUserDetails
            {
                Id = model.Id,
                Email = model.Email,
                GovUkUser = govUkUser
            });

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claims,
                new AuthenticationProperties());

            return RedirectToRoute("SignedIn-stub", new { model.ReturnUrl });
        }

        [Authorize]
        [HttpGet]
        [Route("signed-in-stub", Name = "SignedIn-stub")]
        public IActionResult SignedInStub(string returnUrl)
        {
            return View(model: returnUrl);
        }
#endif
    }
}
