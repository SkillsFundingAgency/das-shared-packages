using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.DfESignIn.Auth.Configuration;
using SFA.DAS.DfESignIn.Auth.Enums;
using SFA.DAS.DfESignIn.Auth.Extensions;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using SFA.DAS.DfESignIn.Auth.Services;
using System;

namespace SFA.DAS.DfESignIn.Auth.AppStart
{
    /// <summary>
    /// Registers DfE Sign-in using the legacy ClientName enum.
    /// </summary>
    /// <remarks>
    /// Client configuration selection is coupled to the package-owned ClientName
    /// enum. Adding a new client therefore requires a shared-package change and
    /// release. Use the overload accepting clientConfigurationSectionName so
    /// applications can select their configuration without extending the enum.
    /// </remarks>
    [Obsolete(
        "Use the overload accepting clientConfigurationSectionName. " +
        "This overload couples client selection to the package-owned ClientName enum, " +
        "requiring a shared-package change and release for each new client.")]
    public static class AddAndConfigureDfESignInAuthenticationExtension
    {
        public static void AddAndConfigureDfESignInAuthentication(
            this IServiceCollection services,
            IConfiguration configuration,
            string authenticationCookieName,
            Type customServiceRole,
            ClientName clientName,
            string signOutPath = "/signed-out",
            string redirectUrl = "")
        {
            var useStub = !string.IsNullOrEmpty(configuration["NoAuthEmail"]);

            services.AddServiceRegistration(configuration, customServiceRole, clientName, useStub);

            if (useStub)
            {
                services.AddProviderStubAuthentication($"{authenticationCookieName}.stub", signOutPath,configuration["ResourceEnvironmentName"], clientName);
            }
            else
            {
                services.ConfigureDfESignInAuthentication(configuration, authenticationCookieName, clientName, signOutPath, redirectUrl.GetSignedOutRedirectUrl(configuration["ResourceEnvironmentName"], clientName));
            }
        }

        public static void AddAndConfigureDfESignInAuthentication(
            this IServiceCollection services,
            IConfiguration configuration,
            string authenticationCookieName,
            Type customServiceRole,
            string clientConfigurationSectionName,
            string signOutPath = "/signed-out")
        {
            services.AddServiceRegistration(configuration, customServiceRole, clientConfigurationSectionName);
            services.Configure<DfESignInStubConfiguration>(configuration.GetSection(nameof(DfESignInStubConfiguration)));
            services.AddTransient<IDfESignInStubService, DfESignInStubService>();

            var oidcConfiguration = configuration.GetSection(nameof(DfEOidcConfiguration)).Get<DfEOidcConfiguration>();
            configuration.GetSection(clientConfigurationSectionName).Bind(oidcConfiguration);

            bool.TryParse(configuration["StubAuth"], out var stubAuth);

            if (stubAuth && !string.Equals(configuration["ResourceEnvironmentName"], "PRD", StringComparison.OrdinalIgnoreCase))
            {
                services.ConfigureDfESignInStubAuthentication(
                    oidcConfiguration,
                    authenticationCookieName,
                    signOutPath);
            }
            else
            {
                services.ConfigureDfESignInAuthentication(
                    oidcConfiguration,
                    authenticationCookieName,
                    signOutPath);
            }
        }
    }
}