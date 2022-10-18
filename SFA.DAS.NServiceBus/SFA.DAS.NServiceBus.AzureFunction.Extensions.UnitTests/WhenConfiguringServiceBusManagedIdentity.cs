using FluentAssertions;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.NServiceBus.AzureFunction.Extensions.UnitTests;

public class WhenConfiguringServiceBusManagedIdentity
{
    [Test]
    public void Then_should_create_in_memory_collection()
    {
        var settings = new Dictionary<string, string>
        {
            {"AzureWebJobsServiceBus__fullyQualifiedNamespace", "abc.xyz.com"}
        };

        var configBuilder = new ConfigurationBuilder().AddInMemoryCollection(settings);
        var functionConfigBuilderMock = new Mock<IFunctionsConfigurationBuilder>();
        functionConfigBuilderMock.SetupGet(x => x.ConfigurationBuilder).Returns(configBuilder);

        var sut = functionConfigBuilderMock.Object;
        sut.ConfigureServiceBusManagedIdentity();

        var config = configBuilder.Build();

        config["AzureWebJobsServiceBus__fullyQualifiedNamespace"].Should().Be("abc.xyz.com");
        config["AzureWebJobsServiceBus"].Should().Be($"Endpoint=sb://abc.xyz.com/;Authentication=Managed Identity;");
    }
}