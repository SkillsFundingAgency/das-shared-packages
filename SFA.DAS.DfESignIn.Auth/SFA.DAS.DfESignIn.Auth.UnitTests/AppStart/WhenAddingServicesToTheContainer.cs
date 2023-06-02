using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SFA.DAS.DfESignIn.Auth.AppStart;
using SFA.DAS.DfESignIn.Auth.Interfaces;
using SFA.DAS.DfESignIn.Auth.Configuration;
using SFA.DAS.DfESignIn.Auth.Api.Helpers;
using SFA.DAS.DfESignIn.Auth.Enums;

namespace SFA.DAS.DfESignIn.Auth.UnitTests.AppStart;


public class WhenAddingServicesToTheContainer
{
    private const string ClientName = "someProvider";

    [TestCase(typeof(DfEOidcConfiguration))]
    [TestCase(typeof(IDfESignInService))]
    [TestCase(typeof(ITokenDataSerializer))]
    [TestCase(typeof(ITokenBuilder))]
    [TestCase(typeof(IApiHelper))]
    public void Then_The_Dependencies_Are_Correctly_Resolved(Type toResolve)
    {
        var serviceCollection = new ServiceCollection();
        SetupServiceCollection(serviceCollection);

        var provider = serviceCollection.BuildServiceProvider();

        var type = provider.GetService(toResolve);

        Assert.That(type, Is.Not.Null);
    }

    [Test]
    [TestCase(null)]
    [TestCase("")]
    public void Then_The_ClientName_Given_NullOrEmpty_Throw_Exception(string clientName)
    {
        // arrange
        var configuration = GenerateConfiguration();
        var serviceCollection = new ServiceCollection();

        // sut
        Assert.Throws<ArgumentNullException>(() => 
            serviceCollection.AddServiceRegistration(configuration, typeof(TestCustomServiceRole), clientName));
    }

    [Test]
    public void Then_The_Configuration_Given_NullOrEmpty_Throw_Exception()
    {
        // arrange
        var configuration = new ConfigurationRoot(new List<IConfigurationProvider>
        { 
            new MemoryConfigurationProvider(new MemoryConfigurationSource
            {
                InitialData = new List<KeyValuePair<string, string>>()

            })
        });
        var serviceCollection = new ServiceCollection();

        // sut
        Assert.Throws<ArgumentException>(() =>
            serviceCollection.AddServiceRegistration(configuration, typeof(TestCustomServiceRole), It.IsAny<string>()));
    }

    private static void SetupServiceCollection(IServiceCollection serviceCollection)
    {
        var configuration = GenerateConfiguration();
        serviceCollection.AddServiceRegistration(configuration, typeof(TestCustomServiceRole), ClientName);
    }

    private static IConfigurationRoot GenerateConfiguration()
    {
        var configSource = new MemoryConfigurationSource
        {
            InitialData = new List<KeyValuePair<string, string>>
            {
                new("DfEOidcConfiguration:BaseUrl", "https://test.com/"),
                new("DfEOidcConfiguration:ClientId", "1234567"),
                new("DfEOidcConfiguration:APIServiceSecret", "1234567"),
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

    public class TestCustomServiceRole : ICustomServiceRole
    {
        public string RoleClaimType => throw new NotImplementedException();
        public CustomServiceRoleValueType RoleValueType => CustomServiceRoleValueType.Name;
    }
}