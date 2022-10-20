using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NUnit.Framework;

namespace SFA.DAS.NServiceBus.AzureFunction.Extensions.UnitTests;
public class WhenCreatingSingleQueueEndpoint
{
    [Test, AutoData]
    public void Should_create_a_single_queue_endpoint()
    {
        var config = (new ConfigurationBuilder()).Build();
        var endpoint = ServiceBusEndpointFactory.CreateSingleQueueConfiguration("QueueName", config);

        endpoint.Should().NotBeNull();
    }
}