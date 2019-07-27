using System.IO;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using DfE.Example.Web;

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
    }
}
