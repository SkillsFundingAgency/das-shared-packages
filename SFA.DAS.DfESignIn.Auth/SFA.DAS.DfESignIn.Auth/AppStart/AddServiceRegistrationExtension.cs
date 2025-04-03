using System;
using System.Linq;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Extensions.Http;
using SFA.DAS.DfESignIn.Auth.Api.Client;
using SFA.DAS.DfESignIn.Auth.Api.Helpers;
using SFA.DAS.DfESignIn.Auth.Configuration;
using SFA.DAS.DfESignIn.Auth.Enums;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using SFA.DAS.DfESignIn.Auth.Services;

namespace SFA.DAS.DfESignIn.Auth.AppStart
{
    internal static class AddServiceRegistrationExtension
    {
        internal static void AddServiceRegistration(
            this IServiceCollection services, 
            IConfiguration configuration, 
            Type customServiceRole, 
            ClientName clientName)
        {
            if (!configuration.GetSection(nameof(DfEOidcConfiguration)).GetChildren().Any())
            {
                throw new ArgumentException(
                    "Cannot find DfEOidcConfiguration in configuration. Please add a section called DfESignInOidcConfiguration with BaseUrl, ClientId and Secret properties.");
            }

            var configName = clientName;
            if (clientName == ClientName.RoatpServiceAdmin)
            {
                configName = ClientName.ServiceAdmin;
            }
            
            services.AddOptions();
            services.Configure<DfEOidcConfiguration>(configuration.GetSection(nameof(DfEOidcConfiguration)));
            services.Configure<DfEOidcConfiguration>(configuration.GetSection($"{nameof(DfEOidcConfiguration)}_{configName}"));

            services.AddSingleton(cfg => cfg.GetService<IOptions<DfEOidcConfiguration>>().Value);
            services.AddTransient(typeof(ICustomServiceRole), customServiceRole);
            services.AddTransient<IDfESignInService, DfESignInService>();
            services.AddHttpClient<IApiHelper, DfeSignInApiHelper>
                (
                    options => options.Timeout = TimeSpan.FromMinutes(30)
                )
                .SetHandlerLifetime(TimeSpan.FromMinutes(10))
                .AddPolicyHandler(HttpClientRetryPolicy());
            services.AddTransient<ITokenDataSerializer, TokenDataSerializer>();
            services.AddTransient<ITokenBuilder, TokenBuilder>();
            services.AddSingleton<ITicketStore, AuthenticationTicketStore>();

            var connection = configuration.GetSection(nameof(DfEOidcConfiguration)).Get<DfEOidcConfiguration>();
            if (string.IsNullOrEmpty(connection.DfELoginSessionConnectionString))
            {
                services.AddDistributedMemoryCache();
            }
            else
            {
                services.AddStackExchangeRedisCache(options =>
                {
                    options.Configuration = connection.DfELoginSessionConnectionString;
                });
            }
        }

        private static IAsyncPolicy<HttpResponseMessage> HttpClientRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                    retryAttempt)));
        }
    }
}