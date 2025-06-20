using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SFA.DAS.GovUK.Auth.Services;

[assembly: Microsoft.AspNetCore.Mvc.ApplicationParts.ProvideApplicationPartFactory(
    typeof(Microsoft.AspNetCore.Mvc.ApplicationParts.CompiledRazorAssemblyApplicationPartFactory))]

namespace SFA.DAS.GovUK.Auth.Controllers
{

    [Route("service/verify-identity")]
    public class VerifyIdentityController : Controller
    {
        private readonly IGovUkAuthenticationService _govUkAuthenticationService;

        public VerifyIdentityController(IGovUkAuthenticationService govUkAuthenticationService)
        {
            _govUkAuthenticationService = govUkAuthenticationService;
        }

        [HttpGet]
        public Task<IActionResult> Index(string returnUrl = "/")
        {
            return _govUkAuthenticationService.ChallengeWithVerifyAsync(returnUrl, this);
        }
    }
}