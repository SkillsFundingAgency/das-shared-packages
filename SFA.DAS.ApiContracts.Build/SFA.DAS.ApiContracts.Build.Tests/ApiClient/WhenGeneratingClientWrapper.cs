using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ApiContracts.Build.Tests.Client;

namespace SFA.DAS.ApiContracts.Build.Tests.ApiClient;

public class WhenGeneratingClientWrapper
{
    [Test]
    public void ConfigurationClass_ImplementsIInternalApiConfiguration()
    {
        typeof(TestApiConfiguration).Should().Implement<IInternalApiConfiguration>();
    }

    [Test]
    public void ConfigurationClass_IsNamedAfterClientPlusConfiguration()
    {
        typeof(TestApiConfiguration).Name.Should().Be("TestApiConfiguration");
    }

    [Test]
    public void Interface_ExtendsIInternalApiClient()
    {
        typeof(ITestApiClient<>).GetInterfaces()
            .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IInternalApiClient<>))
            .Should().BeTrue();
    }

    [Test]
    public void ConcreteClass_ImplementsTypedInterface()
    {
        typeof(TestApiClient).Should()
            .Implement<ITestApiClient<TestApiConfiguration>>();
    }

    [Test]
    public void ExtensionMethod_RegistersClientDescriptor()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["TestApiConfiguration:Url"]        = "https://example.com",
                ["TestApiConfiguration:Identifier"] = "test-id",
            })
            .Build();

        var services = new ServiceCollection();
        services.AddTestApiClient(config);

        services.Should().ContainSingle(sd =>
            sd.ServiceType == typeof(ITestApiClient<TestApiConfiguration>) &&
            sd.ImplementationType == typeof(TestApiClient));
    }

    [Test]
    public void ExtensionMethod_ThrowsWhenConfigSectionMissing()
    {
        var config = new ConfigurationBuilder().Build();
        var services = new ServiceCollection();

        var act = () => services.AddTestApiClient(config);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*TestApiConfiguration*");
    }
}
