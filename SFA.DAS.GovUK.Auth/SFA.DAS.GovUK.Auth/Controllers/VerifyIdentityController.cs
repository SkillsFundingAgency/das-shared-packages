using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.GovUK.Auth.Controllers
{
    [Route("service/verify-identity")]
    public class VerifyIdentityController : Controller
    {
        [HttpGet]
        public IActionResult Index(string returnUrl = "/")
        {
            var props = new AuthenticationProperties { RedirectUri = returnUrl };
            props.Items["vtr"] = JsonSerializer.Serialize(new[] { "Cl.Cm.P2" });
            props.Items["claims"] = JsonSerializer.Serialize(new Dictionary<string, object>
            {
                ["userinfo"] = new Dictionary<string, object>
                {
                    ["https://vocab.account.gov.uk/v1/coreIdentityJWT"] = null,
                    ["https://vocab.account.gov.uk/v1/address"] = null,
                    ["https://vocab.account.gov.uk/v1/passport"] = null,
                    ["https://vocab.account.gov.uk/v1/drivingPermit"] = null
                }
            });

            props.Items["isVerification"] = true.ToString();
            return Challenge(props, OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}