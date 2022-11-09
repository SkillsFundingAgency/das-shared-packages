using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Notifications;
using Newtonsoft.Json;
using OpenAthens.Owin.Security.OpenIdConnect;
using Owin;
using SFA.DAS.DfESignIn.SampleSite.Framework.Api;
using SFA.DAS.DfESignIn.SampleSite.Framework.Api.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SFA.DAS.DfESignIn.SampleSite.Framework
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            string oidcRedirectUrl = "https://localhost:44300/sign-in";

            var oidcOptions = new OpenIdConnectAuthenticationOptions
            {
                Authority = ConfigurationManager.AppSettings["BaseUrl"],
                ClientId = ConfigurationManager.AppSettings["ClientId"],
                ClientSecret = ConfigurationManager.AppSettings["Secret"],
                GetClaimsFromUserInfoEndpoint = true,
                PostLogoutRedirectUri = oidcRedirectUrl,
                RedirectUri = oidcRedirectUrl,
                ResponseType = "code",
                Scope = "openid email profile organisation organisationid",
                Notifications = new OpenIdConnectAuthenticationNotifications()
                {
                    SecurityTokenValidated = async n =>
                    {

                        await PopulateAccountsClaim(n);
                    }
                }
            };
            app.UseOpenIdConnectAuthentication(oidcOptions);
        }

        private async Task PopulateAccountsClaim(SecurityTokenValidatedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> n)
        {
            var claimIdentity = new ClaimsIdentity(n.AuthenticationTicket.Identity);

            string userId = claimIdentity.Claims.Where(c => c.Type.Contains("nameidentifier")).Select(c => c.Value).SingleOrDefault();

            var userOrganisation = JsonConvert.DeserializeObject<Organisation>
            (
                claimIdentity.Claims.Where(c => c.Type == "organisation")
                .Select(c => c.Value)
                .FirstOrDefault()
            );

            var ukPrn = userOrganisation.UkPrn ?? 10000531;

            await DfEPublicApi(n, userId, userOrganisation.Id.ToString());

            var displayName = n.AuthenticationTicket.Identity.Claims.FirstOrDefault(c => c.Type.Contains("givenname")).Value + " " + n.AuthenticationTicket.Identity.Claims.FirstOrDefault(c => c.Type.Contains("surname")).Value;
            System.Web.HttpContext.Current.Items.Add(ClaimsIdentity.DefaultNameClaimType, ukPrn);
            System.Web.HttpContext.Current.Items.Add("http://schemas.portal.com/displayname", displayName);
            n.AuthenticationTicket.Identity.AddClaim(new Claim(ClaimsIdentity.DefaultNameClaimType, ukPrn.ToString()));
            n.AuthenticationTicket.Identity.AddClaim(new Claim("http://schemas.portal.com/displayname", displayName));
            n.AuthenticationTicket.Identity.AddClaim(new Claim("http://schemas.portal.com/ukprn", ukPrn.ToString()));
        }

        private static async Task DfEPublicApi(SecurityTokenValidatedNotification<OpenIdConnectMessage, OpenIdConnectAuthenticationOptions> n, string userId, string userOrgId)
        {
            var clientFactory = new DfESignInClientFactory(ConfigurationBuilderExtension.GetConfigurationRoot());
            DfESignInClient dfeSignInClient = clientFactory.CreateDfESignInClient(userId, userOrgId);
            HttpResponseMessage response = await dfeSignInClient.HttpClient.GetAsync(dfeSignInClient.TargetAddress);

            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStringAsync();

                var apiServiceResponse = JsonConvert.DeserializeObject<ApiServiceResponse>(stream);
                var roleClaims = new List<Claim>();
                foreach (var role in apiServiceResponse.Roles)
                {
                    if (role.Status.Id.Equals(1))
                    {
                        // add to current identity because you cannot have multiple identities
                        n.AuthenticationTicket.Identity.AddClaim(new Claim("rolecode", role.Code, ClaimTypes.Role, n.Options.ClientId));
                        n.AuthenticationTicket.Identity.AddClaim(new Claim("roleId", role.Id.ToString(), ClaimTypes.Role, n.Options.ClientId));
                        n.AuthenticationTicket.Identity.AddClaim(new Claim("roleName", role.Name, ClaimTypes.Role, n.Options.ClientId));
                        n.AuthenticationTicket.Identity.AddClaim(new Claim("rolenumericid", role.NumericId.ToString(), ClaimTypes.Role, n.Options.ClientId));
                    }
                }
            }
        }
    }
}