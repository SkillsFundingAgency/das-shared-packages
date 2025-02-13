using System;
using Microsoft.AspNetCore.Authentication;

namespace SFA.DAS.Employer.Shared.UI.IntegrationTests.Authentication;

public static class TestAuthenticationExtensions
{
    public static AuthenticationBuilder AddTestAuthentication(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<TestAuthenticationOptions> configureOptions)
    {
        return builder.AddScheme<TestAuthenticationOptions, TestAuthHandler>(authenticationScheme, configureOptions);
    }
}