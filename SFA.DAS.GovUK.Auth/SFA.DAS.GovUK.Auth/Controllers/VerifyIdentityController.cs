using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

[assembly: Microsoft.AspNetCore.Mvc.ApplicationParts.ProvideApplicationPartFactory(
    typeof(Microsoft.AspNetCore.Mvc.ApplicationParts.CompiledRazorAssemblyApplicationPartFactory))]

namespace SFA.DAS.GovUK.Auth.Controllers
{

    [Route("service/verify-identity")]
    public class VerifyIdentityController : Controller
    {
        [HttpGet]
        public IActionResult Index(string returnUrl = "/")
        {
            var props = new AuthenticationProperties { RedirectUri = returnUrl, AllowRefresh = true };
            props.Items["enableVerify"] = true.ToString();
            return Challenge(props, OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}