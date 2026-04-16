using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.ApiContracts.Build.Tests.Override.Client;

namespace SFA.DAS.ApiContracts.Build.Tests.ApiClient;

public class WhenGeneratingClientWrapperWithConfigurationNameOverride
{
    [Test]
    public void ConfigurationClass_UsesOverriddenName()
    {
        typeof(OverriddenApiConfiguration).Name.Should().Be("OverriddenApiConfiguration");
    }

    [Test]
    public void ConfigurationClass_ImplementsIInternalApiConfiguration()
    {
        typeof(OverriddenApiConfiguration).Should().Implement<IInternalApiConfiguration>();
    }

    [Test]
    public void ConcreteClass_UsesOverriddenConfigurationType()
    {
        typeof(TestApiClient).Should()
            .Implement<ITestApiClient<OverriddenApiConfiguration>>();
    }

    [Test]
    public void ExtensionMethod_RegistersClientDescriptorWithOverriddenConfigType()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["OverriddenApiConfiguration:Url"]        = "https://example.com",
                ["OverriddenApiConfiguration:Identifier"] = "override-id",
            })
            .Build();

        var services = new ServiceCollection();
        services.AddTestApiClient(config);

        services.Should().ContainSingle(sd =>
            sd.ServiceType == typeof(ITestApiClient<OverriddenApiConfiguration>) &&
            sd.ImplementationType == typeof(TestApiClient));
    }

    [Test]
    public void ExtensionMethod_ThrowsWhenOverriddenConfigSectionMissing()
    {
        // Providing the default-named section must NOT satisfy the override client —
        // it expects "OverriddenApiConfiguration", not "TestApiConfiguration".
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["TestApiConfiguration:Url"]        = "https://example.com",
                ["TestApiConfiguration:Identifier"] = "test-id",
            })
            .Build();

        var services = new ServiceCollection();

        var act = () => services.AddTestApiClient(config);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*OverriddenApiConfiguration*");
    }
}
