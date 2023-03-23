using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SFA.DAS.GovUK.Auth.Configuration;
using SFA.DAS.GovUK.Auth.Models;

namespace SFA.DAS.GovUK.Auth.Services
{
    public interface IStubAuthenticationService
    {
        void AddStubEmployerAuth(IResponseCookies cookies, StubAuthUserDetails model);
    }

    public class StubAuthenticationService : IStubAuthenticationService
    {
        private readonly string _environment;

        public StubAuthenticationService(IConfiguration configuration)
        {
            _environment = configuration["ResourceEnvironmentName"]?.ToUpper();
        }

        public void AddStubEmployerAuth(IResponseCookies cookies, StubAuthUserDetails model)
        {
            if (_environment == "PRD")
            {
                return;
            }
            
            var authCookie = new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddMinutes(10),
                Path = "/",
                Domain = _environment != "LOCAL" ? $".{_environment.ToLower()}-eas.apprenticeships.education.gov.uk" : "localhost",
                Secure = false,
                HttpOnly = false,
                IsEssential = false,
                SameSite = SameSiteMode.None
            };
            cookies.Append(GovUkConstants.StubAuthCookieName, JsonConvert.SerializeObject(model), authCookie);

            
        }
    }
}