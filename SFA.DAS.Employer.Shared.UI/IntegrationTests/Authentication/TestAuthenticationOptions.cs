using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using System;

namespace SFA.DAS.Employer.Shared.UI.IntegrationTests
{
    public class TestAuthenticationOptions : AuthenticationSchemeOptions
    {
        public bool RequireBearerToken { get; set; }
        // customize as needed
        public virtual ClaimsIdentity Identity { get; set; } = new ClaimsIdentity(
            new Claim[]
                {
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier", Guid.NewGuid().ToString()),
                new Claim("http://schemas.microsoft.com/identity/claims/tenantid", "test"),
                new Claim("http://schemas.microsoft.com/identity/claims/objectidentifier", Guid.NewGuid().ToString()),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname", "test"),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/surname", "test"),
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/upn", "test"),
                },
            "test");

        public TestAuthenticationOptions() { }
    }
}
