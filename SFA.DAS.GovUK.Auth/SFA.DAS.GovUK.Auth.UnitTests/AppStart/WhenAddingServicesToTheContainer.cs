using System.Security.Claims;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.GovUK.Auth.AppStart;
using SFA.DAS.GovUK.Auth.Authentication;
using SFA.DAS.GovUK.Auth.Services;

namespace SFA.DAS.GovUK.Auth.UnitTests.AppStart;

public class WhenAddingServicesToTheContainer
{
    [TestCase(typeof(IOidcService))]
    [TestCase(typeof(IAzureIdentityService))]
    [TestCase(typeof(IJwtSecurityTokenService))]
    [TestCase(typeof(ICustomClaims))]
    [TestCase(typeof(IStubAuthenticationService))]
    public void Then_The_Dependencies_Are_Correctly_Resolved(Type toResolve)
    {
        var serviceCollection = new ServiceCollection();
        SetupServiceCollection(serviceCollection);
        
        var provider = serviceCollection.BuildServiceProvider();

        var type = provider.GetService(toResolve);
            
        Assert.That(type, Is.Not.Null);
    }
    [Test]
    public void Then_Resolves_Authorization_Handlers()
    {
        var serviceCollection = new ServiceCollection();
        SetupServiceCollection(serviceCollection);
        var provider = serviceCollection.BuildServiceProvider();
            
        var type = provider.GetServices(typeof(IAuthorizationHandler)).ToList();
            
        Assert.That(type, Is.Not.Null);
        type.Count.Should().Be(1);
        type.Should().ContainSingle(c => c.GetType() == typeof(AccountActiveAuthorizationHandler));
    }
    

    private static void SetupServiceCollection(IServiceCollection serviceCollection)
    {   
        var configuration = GenerateConfiguration();
        serviceCollection.AddSingleton<IConfiguration>(configuration);
        serviceCollection.AddServiceRegistration(configuration,typeof(TestCustomClaims));
    }

    private static IConfigurationRoot GenerateConfiguration()
    {
        var configSource = new MemoryConfigurationSource
        {
            InitialData = new List<KeyValuePair<string, string>>
            {
                new("GovUkOidcConfiguration:BaseUrl", "https://test.com/"),
                new("GovUkOidcConfiguration:ClientId", "1234567"),
                new("GovUkOidcConfiguration:KeyVaultIdentifier", "https://test.com/"),
                new("ResourceEnvironmentName", "AT")
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