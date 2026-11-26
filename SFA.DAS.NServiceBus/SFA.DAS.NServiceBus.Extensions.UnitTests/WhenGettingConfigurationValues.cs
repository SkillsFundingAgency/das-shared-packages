using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.NServiceBus.Extensions.UnitTests;

public class WhenGettingConfigurationValues
{
    [Test]
    public void Then_NServiceBusConnectionString_should_return_connection()
    {
        var connectionString = "testConnectionString";
        var config = new Mock<IConfiguration>();
        config.Setup(x => x["NServiceBusConnectionString"]).Returns(connectionString);

        ConfigurationExtensions.NServiceBusConnectionString(config.Object).Should().Be(connectionString);
    }

    [Test]
    public void Then_NServiceBusConnectionString_should_return_default_connection()
    {
        var config = new Mock<IConfiguration>();

        ConfigurationExtensions.NServiceBusConnectionString(config.Object).Should().Be("UseLearningEndpoint=true");
    }

    [Test]
    public void Then_NServiceBusLicense_should_returned()
    {
        var license = "testConnectionString";
        var config = new Mock<IConfiguration>();
        config.Setup(x => x["NServiceBusLicense"]).Returns(license);

        ConfigurationExtensions.NServiceBusLicense(config.Object).Should().Be(license);
    }

    [Test]
    public void Then_NServiceBusLicense_should_return_null_if_not_set()
    {
        var config = new Mock<IConfiguration>();

        ConfigurationExtensions.NServiceBusLicense(config.Object).Should().BeNull();
    }

    [Test]
    public void Then_NServiceBusFullNamespace_should_return_connection()
    {
        var connectionString = "testConnectionString";
        var config = new Mock<IConfiguration>();
        config.Setup(x => x["AzureWebJobsServiceBus:fullyQualifiedNamespace"]).Returns(connectionString);

        ConfigurationExtensions.NServiceBusFullNamespace(config.Object).Should().Be(connectionString);
    }

    [Test]
    public void Then_NServiceBusSASConnectionString_should_return_connection()
    {
        var connectionString = "testConnectionString";
        var config = new Mock<IConfiguration>();
        config.Setup(x => x["AzureWebJobsServiceBus"]).Returns(connectionString);

        ConfigurationExtensions.NServiceBusSASConnectionString(config.Object).Should().Be(connectionString);
    }
}
