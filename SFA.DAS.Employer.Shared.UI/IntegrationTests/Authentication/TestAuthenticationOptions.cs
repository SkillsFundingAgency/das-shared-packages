using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace SFA.DAS.Employer.Shared.UI.IntegrationTests.Authentication;

public sealed class TestAuthenticationOptions : AuthenticationSchemeOptions
{
    // customize as needed
    public ClaimsIdentity Identity { get; } = new(
        new Claim[]
        {
            new("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", Guid.NewGuid().ToString()),
            new("http://schemas.microsoft.com/identity/claims/tenantid", "test"),
            new("http://schemas.microsoft.com/identity/claims/objectidentifier", Guid.NewGuid().ToString()),
            new("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname", "test"),
            new("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname", "test"),
            new("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", "test"),
        }, "test");
}