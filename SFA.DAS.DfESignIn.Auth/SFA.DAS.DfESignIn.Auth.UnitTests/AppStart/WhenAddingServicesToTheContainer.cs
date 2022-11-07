using FluentAssertions.Common;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.DfESignIn.Auth.AppStart;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using SFA.DAS.DfESignIn.SampleSite.AppStart;
using System.Security.Claims;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.AppStart;

public class WhenAddingServicesToTheContainer
{
    [TestCase(typeof(ICustomClaims))]
    public void Then_The_Dependencies_Are_Correctly_Resolved(Type toResolve)
    {
        var serviceCollection = new ServiceCollection();
        SetupServiceCollection(serviceCollection);
        
        var provider = serviceCollection.BuildServiceProvider();

        var type = provider.GetService(toResolve);

        Assert.That(type, Is.Not.Null);
    }

    private static void SetupServiceCollection(IServiceCollection serviceCollection)
    {   
        var configuration = GenerateConfiguration();
        serviceCollection.AddServiceRegistration(configuration);
    }

    private static IConfigurationRoot GenerateConfiguration()
    {
        var configSource = new MemoryConfigurationSource
        {
            InitialData = new List<KeyValuePair<string, string>>
            {
                new("DfEOidcConfiguration:BaseUrl", "https://test.com/"),
                new("DfEOidcConfiguration:ClientId", "1234567"),
                new("ProviderSharedUIConfiguration:DashboardUrl", "https://localhost:7136")
            }
        };

        var provider = new MemoryConfigurationProvider(configSource);

        return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
    }
    
    public class TestCustomClaims : ICustomClaims
    {
        public Task<IEnumerable<Claim?>> GetClaims(TokenValidatedContext tokenValidatedContext)
        {
            throw new NotImplementedException();
        }
    }
}