using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.NServiceBus.Extensions.UnitTests;

public class WhenGettingConfigurationValues
{
    [Test, AutoData]
    public void Then_NServiceBusConnectionString_should_return_connection(string connection)
    {
        var config = new Mock<IConfiguration>();
        config.Setup(x => x["NServiceBusConnectionString"]).Returns(connection);

        ConfigurationExtensions.NServiceBusConnectionString(config.Object).Should().Be(connection);
    }

    [Test]
    public void Then_NServiceBusConnectionString_should_return_default_connection()
    {
        var config = new Mock<IConfiguration>();

        ConfigurationExtensions.NServiceBusConnectionString(config.Object).Should().Be("UseLearningEndpoint=true");
    }

    [Test, AutoData]
    public void Then_NServiceBusLicense_should_returned(string license)
    {
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
}
