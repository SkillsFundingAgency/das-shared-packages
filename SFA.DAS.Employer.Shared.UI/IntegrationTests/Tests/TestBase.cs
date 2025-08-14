using System.IO;
using DfE.Example.Web;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using SFA.DAS.Employer.Shared.UI.IntegrationTests.Authentication;

namespace SFA.DAS.Employer.Shared.UI.IntegrationTests.Tests;

public abstract class TestBase 
{
    private WebApplicationFactory<Startup> _factory;

    [SetUp]
    public void SetUp()
    {
        var factory = new WebApplicationFactory<Startup>();
            
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((_, config) =>
            {
                config.Sources.Clear();
            
                config.SetBasePath(Directory.GetCurrentDirectory());
                config.AddJsonFile("appSettings.json", optional: false, reloadOnChange: false);
                config.AddJsonFile("sharedMenuSettings.json", optional: false, reloadOnChange: false);
            });
        });
    }

    protected System.Net.Http.HttpClient BuildClient(bool authenticated = true)
    {
        return _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((_, config) =>
                    {
                        config.Sources.Clear();

                        config.SetBasePath(Directory.GetCurrentDirectory());
                        config.AddJsonFile("appSettings.json", optional: false, reloadOnChange: false);
                        config.AddJsonFile("sharedMenuSettings.json", optional: false, reloadOnChange: false);
                    })
                    .ConfigureTestServices(services =>
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
                AllowAutoRedirect = true,
            });
    }
}