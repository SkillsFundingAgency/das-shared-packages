using System.IO;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using DfE.Example.Web;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading.Tasks;
using System.Security.Claims;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authentication;
using System.Linq;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Text.Encodings.Web;
using System;

namespace SFA.DAS.Employer.Shared.UI.IntegrationTests
{
    public abstract class TestBase : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        protected TestBase(WebApplicationFactory<Startup> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((hostingContext, config) =>
                {
                    config.Sources.Clear();

                    config.SetBasePath(Directory.GetCurrentDirectory());
                    config.AddJsonFile("appSettings.json", optional: false, reloadOnChange: false);
                    config.AddJsonFile("sharedMenuSettings.json", optional: false, reloadOnChange: false);
                    config.AddJsonFile("linkGeneratorSettings.json", optional: false, reloadOnChange: false);
                });
            });
        }

        protected System.Net.Http.HttpClient BuildClient(bool authenticated = true)
        {
            return _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddMvc(options =>
                    {
                        if (!authenticated)
                            options.Filters.Add(new AllowAnonymousFilter());
                    });

                    if (authenticated)
                    {
                        services.AddAuthentication(o =>
                        {
                            o.DefaultScheme = "TestAuthenticationScheme";
                        })
                        .AddTestAuthentication("TestAuthenticationScheme", "Test Auth",  o => { });
                    }
                });
            })
            .CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false
            });
        }

        private class FakeUserFilter : IAsyncActionFilter
        {
            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, "12345678-1234-1234-1234-123456789012"),
                    new Claim(ClaimTypes.Name, "TestUser"),
                    new Claim(ClaimTypes.Email, "test.user@example.com"), // add as many claims as you need
                }));

                await next();
            }
        }

        
    }

        public class TestAuthHandler : AuthenticationHandler<TestAuthenticationOptions>
    {
        public TestAuthHandler(IOptionsMonitor<TestAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            // if the header is provided and its value is 'true', then step aside and let the regular auth handle it
            // a value of <anything other than 'true'> will bypass the rest of the auth pipeline
            // if (!Context.Request.Headers.TryGetValue("X-Test-Auth", out var header) || header.Contains("true"))
            // {
            //     return Task.FromResult(AuthenticateResult.NoResult());
            // }
            // value is false (or anything other than 'true'). Make sure they've made the effort to provide the auth header 
            // but don't bother to check the value
            // if (header.Any() && !Context.Request.Headers.TryGetValue("Authorization", out var authHeader))
            // {
            //     return Task.FromResult(AuthenticateResult.NoResult());
            // }
            // otherwise, here's your auth ticket!
            return Task.FromResult(
             AuthenticateResult.Success(
                new AuthenticationTicket(
                    new ClaimsPrincipal(Options.Identity),
                    new AuthenticationProperties(),
                    this.Scheme.Name)));

        }
    }
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

        public TestAuthenticationOptions() {}
    }


    public static class TestAuthenticationExtensions
    {
        public static AuthenticationBuilder AddTestAuthentication(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<TestAuthenticationOptions> configureOptions)
        {
            return builder.AddScheme<TestAuthenticationOptions, TestAuthHandler>(authenticationScheme, configureOptions);
        }
    }
}
