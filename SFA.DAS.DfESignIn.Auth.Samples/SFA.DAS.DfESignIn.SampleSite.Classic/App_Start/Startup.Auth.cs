using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using OpenAthens.Owin.Security.OpenIdConnect;
using Owin;
using System.Configuration;

namespace SFA.DAS.DfESignIn.SampleSite.Classic
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
                ResponseType = OpenIdConnectResponseType.Code,
                Scope = ConfigurationManager.AppSettings["Scopes"],
                Notifications = new OpenIdConnectAuthenticationNotifications()
                {
                    MessageReceived = n => {
                        return System.Threading.Tasks.Task.CompletedTask;
                    }
                }
            };
            app.UseOpenIdConnectAuthentication(oidcOptions);
        }
    }
}