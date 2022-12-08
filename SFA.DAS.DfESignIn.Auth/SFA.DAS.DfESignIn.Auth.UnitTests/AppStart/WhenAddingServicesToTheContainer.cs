using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.DfESignIn.SampleSite.AppStart;
using SFA.DAS.DfESignIn.Auth.AppStart;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using SFA.DAS.DfESignIn.Auth.Configuration;
using Moq;
using SFA.DAS.DfESignIn.Auth.Services;
using SFA.DAS.DfESignIn.Auth.Api.Helpers;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.AppStart;


public class WhenAddingServicesToTheContainer
{
    [TestCase(typeof(DfEOidcConfiguration))]
    [TestCase(typeof(IDfESignInService))]
    [TestCase(typeof(ITokenDataSerializer))]
    [TestCase(typeof(ITokenEncoder))]
    [TestCase(typeof(IJsonWebAlgorithm))]
    [TestCase(typeof(ITokenData))]
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
                new("DfEOidcConfiguration:KeyVaultIdentifier", "https://test.com/"),
                new("ProviderSharedUIConfiguration:DashboardUrl", "https://test.com/")
            }
        };

        var provider = new MemoryConfigurationProvider(configSource);

        return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
    }

    public class TestCustomClaims : ICustomClaims
    {
        public IEnumerable<Claim?> GetClaims(TokenValidatedContext tokenValidatedContext)
        {
            throw new NotImplementedException();
        }
    }
}