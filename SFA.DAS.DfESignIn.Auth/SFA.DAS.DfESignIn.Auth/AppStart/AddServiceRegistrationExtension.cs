using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SFA.DAS.DfESignIn.Auth.Api.Client;
using SFA.DAS.DfESignIn.Auth.Api.Helpers;
using SFA.DAS.DfESignIn.Auth.Configuration;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using SFA.DAS.DfESignIn.Auth.Services;
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
#if NETSTANDARD2_0
            services.Configure<DfEOidcConfiguration>(_=>configuration.GetSection(nameof(DfEOidcConfiguration)));
#else 
            services.Configure<DfEOidcConfiguration>(configuration.GetSection(nameof(DfEOidcConfiguration)));
#endif

            services.AddSingleton(cfg => cfg.GetService<IOptions<DfEOidcConfiguration>>().Value);
            services.AddHttpClient<IDfESignInService, DfESignInService>();
            services.AddTransient<ITokenDataSerializer, TokenDataSerializer>();
            services.AddTransient<ITokenEncoder, TokenEncoder>();
            services.AddTransient<IJsonWebAlgorithm, JsonWebAlgorithm>();
            services.AddTransient<ITokenData, TokenData>();
        }
    }
}