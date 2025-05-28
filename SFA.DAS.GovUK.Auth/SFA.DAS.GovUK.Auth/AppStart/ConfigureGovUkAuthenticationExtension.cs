using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.KeyVaultExtensions;
using Microsoft.IdentityModel.Tokens;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.GovUK.Auth.AppStart
{
    internal static class ConfigureGovUkAuthenticationExtension
    {
        internal static void ConfigureGovUkAuthentication(this IServiceCollection services, IConfiguration configuration, string redirectUrl, string notVerifiedUrl, string cookieDomain)
        {
            services
                .AddAuthentication(sharedOptions =>
                {
                    sharedOptions.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                    sharedOptions.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
                    sharedOptions.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
                }).AddOpenIdConnect(options =>
                {
                    var govUkConfiguration = configuration.GetSection(nameof(GovUkOidcConfiguration));

                    var baseUrl = govUkConfiguration["BaseUrl"];
                    var disable2Fa = govUkConfiguration["Disable2Fa"] != null && govUkConfiguration["Disable2Fa"].Equals( "true", StringComparison.CurrentCultureIgnoreCase);
                    options.ClientId = govUkConfiguration["ClientId"];
                    options.MetadataAddress = $"{baseUrl}/.well-known/openid-configuration";
                    options.ResponseType = "code";
                    options.AuthenticationMethod = OpenIdConnectRedirectBehavior.RedirectGet;
                    options.SignedOutRedirectUri = "/";
                    options.SignedOutCallbackPath = "/signed-out";
                    options.CallbackPath = "/sign-in";
                    options.ResponseMode = string.Empty;

                    options.SaveTokens = true;

                    var scopes = "openid email phone".Split(' ');
                    options.Scope.Clear();
                    foreach (var scope in scopes)
                    {
                        options.Scope.Add(scope);
                    }

                    options.Events.OnRemoteFailure = c =>
                    {
                        var isVerification =
                            c.Properties?.Items.TryGetValue("isVerification", out var flag) == true
                            && string.Equals(flag, true.ToString(), StringComparison.OrdinalIgnoreCase);

                        if (isVerification)
                        {
                            c.Response.Redirect(notVerifiedUrl);
                            c.HandleResponse();
                        }
                        else
                        {
                            if (c.Failure != null && c.Failure.Message.Contains("Correlation failed"))
                            {
                                c.Response.Redirect("/");
                                c.HandleResponse();
                            }
                        }

                        return Task.CompletedTask;
                    };

                    options.Events.OnRedirectToIdentityProvider = async c =>
                    {
                        var isVerification =
                            c.Properties?.Items.TryGetValue("isVerification", out var flag) == true &&
                            bool.TryParse(flag, out var f) && f;

                        if (isVerification)
                        {
                            var props = c.Properties?.Items ?? new Dictionary<string, string>();
                            var vtr = props.TryGetValue("vtr", out var raw) ? JsonSerializer.Deserialize<string[]>(raw) : ["Cl.Cm"];
                            var claims = props.TryGetValue("claims", out var cRaw) ? JsonSerializer.Deserialize<Dictionary<string, object>>(cRaw) : null;

                            var state = c.Options.StateDataFormat.Protect(c.Properties);

                            var responseType = "code";
                            var scope = string.Join(" ", options.Scope);

                            var reqSvc = c.HttpContext.RequestServices.GetRequiredService<IOidcRequestObjectService>();
                            var jwt = await reqSvc.BuildRequestJwtAsync(
                                baseUrl,
                                options.ClientId, 
                                responseType,
                                c.ProtocolMessage.RedirectUri!,
                                scope,
                                state, 
                                c.ProtocolMessage.Nonce!, 
                                vtr, 
                                claims);

                            c.ProtocolMessage.Parameters.Clear();
                            c.ProtocolMessage.ResponseType = responseType;
                            c.ProtocolMessage.Scope = scope;
                            c.ProtocolMessage.ClientId = options.ClientId;
                            c.ProtocolMessage.SetParameter("request", jwt);
                        }
                        else
                        {
                            var stringVector = JsonSerializer.Serialize(new List<string>
                            {
                                disable2Fa ? "Cl" : "Cl.Cm"
                            });

                            if (c.ProtocolMessage.Parameters.ContainsKey("vtr"))
                            {
                                c.ProtocolMessage.Parameters.Remove("vtr");
                            }

                            c.ProtocolMessage.Parameters.Add("vtr", stringVector);
                        }
                    };
                })
                .AddCookie(options =>
                {
                    options.AccessDeniedPath = new PathString("/error/403");
                    
                    options.Cookie.Name = GovUkConstants.AuthCookieName;
                    if (!string.IsNullOrEmpty(cookieDomain))
                    {
                        options.Cookie.Domain = cookieDomain;
                    }
                    options.Cookie.IsEssential = true;
                    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    
                    options.Cookie.SameSite = SameSiteMode.Lax;
                    options.CookieManager = new ChunkingCookieManager { ChunkSize = 3000 };
                    options.LogoutPath = "/home/signed-out";
                })
                ;
            services
                .AddOptions<OpenIdConnectOptions>(OpenIdConnectDefaults.AuthenticationScheme)
                .Configure<IOidcService, IAzureIdentityService, ICustomClaims, GovUkOidcConfiguration, ITicketStore>(
                    (options, oidcService, azureIdentityService, customClaims, config, ticketStore) =>
                    {
                        var govUkConfiguration =config;
                        options.TokenValidationParameters = new TokenValidationParameters
                        {
                            AuthenticationType = "private_key_jwt",
                            IssuerSigningKey = new KeyVaultSecurityKey(govUkConfiguration.KeyVaultIdentifier,
                                azureIdentityService.AuthenticationCallback),
                            ValidateIssuerSigningKey = true,
                            ValidateIssuer = true,
                            ValidateAudience = true,
                            SaveSigninToken = true,
                            ValidIssuer = govUkConfiguration.BaseUrl + "/"
                        };
                        options.Events.OnAuthorizationCodeReceived = async (ctx) =>
                        {
                            var token = await oidcService.GetToken(ctx.TokenEndpointRequest);
                            if (token?.AccessToken!=null && token.IdToken !=null)
                            {
                                ctx.HandleCodeRedemption(token.AccessToken, token.IdToken);
                            }
                        };
                        options.Events.OnSignedOutCallbackRedirect = c =>
                        {
                            c.Response.Cookies.Delete(GovUkConstants.AuthCookieName);
                            c.Response.Redirect(redirectUrl);
                            c.HandleResponse();
                            return Task.CompletedTask;
                        };
                        options.Events.OnTokenValidated = async ctx => await oidcService.PopulateAccountClaims(ctx);
                    });
            services
                .AddOptions<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme)
                .Configure<ITicketStore, GovUkOidcConfiguration>((options, ticketStore, config) =>
                {
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(config.LoginSlidingExpiryTimeOutInMinutes);
                    options.SlidingExpiration = true;
                    options.SessionStore = ticketStore;
                });
            
        }
    }
}