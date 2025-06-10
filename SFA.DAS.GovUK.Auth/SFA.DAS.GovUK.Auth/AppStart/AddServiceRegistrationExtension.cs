using System;
using System.Linq;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.GovUK.Auth.Authentication;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Employer;
using SFA.DAS.GovUK.Auth.Services;
using SFA.DAS.GovUK.Auth.Validation;

namespace SFA.DAS.GovUK.Auth.AppStart
{
    internal static class AddServiceRegistrationExtension
    {
        internal static void AddServiceRegistration(this IServiceCollection services, IConfiguration configuration,
            Type customClaims, Type employerAccountService)
        {
            if (!configuration.GetSection(nameof(GovUkOidcConfiguration)).GetChildren().Any())
            {
                throw new ArgumentException(
                    "Cannot find GovUkOidcConfiguration in configuration. Please add a section called GovUkOidcConfiguration with BaseUrl, ClientId and KeyVaultIdentifier properties.");
            }

            services.AddOptions();
            services.AddHttpContextAccessor();
            
            if (customClaims != null)
            {
                services.AddTransient(typeof(ICustomClaims), customClaims);    
            }
            else
            {
                services.AddTransient<ICustomClaims, EmployerAccountPostAuthenticationClaimsHandler>();    
            }

            if (employerAccountService != null)
            {
                services.AddTransient(typeof(IGovAuthEmployerAccountService), employerAccountService);
            }
            
            services.Configure<GovUkOidcConfiguration>(configuration.GetSection(nameof(GovUkOidcConfiguration)));
            services.AddSingleton(c => c.GetService<IOptions<GovUkOidcConfiguration>>().Value);
            
            services.AddHttpClient<IOidcService, OidcService>();
            services.AddTransient<ISigningCredentialsProvider, AzureKeyVaultSigningCredentialsProvider>();
            services.AddTransient<IOidcRequestObjectService, OidcRequestObjectService>();
            services.AddTransient<IAzureIdentityService, AzureIdentityService>();
            services.AddTransient<IJwtSecurityTokenService, JwtSecurityTokenService>();
            services.AddTransient<IStubAuthenticationService, StubAuthenticationService>();
            
            services.AddSingleton<IAuthorizationHandler, AccountActiveAuthorizationHandler>();
            services.AddSingleton<IAuthorizationHandler, VerifiedIdentityAuthorizationHandler>();
            services.AddSingleton<IAuthorizationMiddlewareResultHandler, RedirectOnVerifiedIdentityFailedResultHandler>();

            services.AddTransient<ValidateCoreIdentityJwtClaimAction>();
            services.AddSingleton<ITicketStore, AuthenticationTicketStore>();
            services.AddSingleton<ICoreIdentityHelper, CoreIdentityHelper>();
            
            var connection = configuration.GetSection(nameof(GovUkOidcConfiguration)).Get<GovUkOidcConfiguration>();
            bool.TryParse(configuration["StubAuth"],out var stubAuth);
            if ((stubAuth && configuration["ResourceEnvironmentName"]!.ToUpper() != "PRD") || string.IsNullOrEmpty(connection.GovLoginSessionConnectionString))
            {
                services.AddDistributedMemoryCache();
            }
            else
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = connection.GovLoginSessionConnectionString;
                });
            }
        }
    }
}