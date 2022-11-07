using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.DfESignIn.Auth.Configuration;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using System;
using System.Linq;

namespace SFA.DAS.DfESignIn.Auth.AppStart
{
    internal static class AddServiceRegistrationExtension
    {
        internal static void AddServiceRegistration(this IServiceCollection services, IConfiguration configuration,
            Type customClaims)
        {
            if (!configuration.GetSection(nameof(DfEOidcConfiguration)).GetChildren().Any())
            {
                throw new ArgumentException(
                    "Cannot find DfEOidcConfiguration in configuration. Please add a section called DfESignInOidcConfiguration with BaseUrl, ClientId and Secret properties.");
            }

            services.AddOptions();
            services.AddTransient(typeof(ICustomClaims), customClaims);
#if NETSTANDARD2_0
            services.Configure<DfEOidcConfiguration>(_=>configuration.GetSection(nameof(DfEOidcConfiguration)));
#else 
            services.Configure<DfEOidcConfiguration>(configuration.GetSection(nameof(DfEOidcConfiguration)));
#endif
            services.AddSingleton(cfg => cfg.GetService<IOptions<DfEOidcConfiguration>>().Value);
        }
    }
}