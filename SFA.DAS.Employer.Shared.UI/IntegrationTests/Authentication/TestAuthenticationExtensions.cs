using Microsoft.AspNetCore.Authentication;
using System;

namespace SFA.DAS.Employer.Shared.UI.IntegrationTests
{
    public static class TestAuthenticationExtensions
    {
        public static AuthenticationBuilder AddTestAuthentication(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<TestAuthenticationOptions> configureOptions)
        {
            return builder.AddScheme<TestAuthenticationOptions, TestAuthHandler>(authenticationScheme, configureOptions);
        }
    }
}
