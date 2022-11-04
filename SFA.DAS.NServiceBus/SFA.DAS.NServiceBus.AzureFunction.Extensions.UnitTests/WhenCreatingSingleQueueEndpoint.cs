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
        var coll = new Dictionary<string, string>
        {
            { "AzureWebJobsStorage", "UseDevelopmentStorage=true;"}
        };
        var config = (new ConfigurationBuilder().AddInMemoryCollection(coll)).Build();
        var endpoint = ServiceBusEndpointFactory.CreateSingleQueueConfiguration("QueueName", config, false);

        endpoint.Should().NotBeNull();
    }

    [Test, AutoData]
    public void Should_create_a_single_queue_endpoint_using_managed_identity()
    {
        var coll = new Dictionary<string, string> 
        { 
            {"AzureWebJobsStorage", "UseDevelopmentStorage=true;"},
            {"AzureWebJobsServiceBus:fullyQualifiedNamespace", "sb://test.com;"},
            {"NServiceBusLicense", "XXXXX"}
        };

        var config = (new ConfigurationBuilder().AddInMemoryCollection(coll)).Build();
        var endpoint = ServiceBusEndpointFactory.CreateSingleQueueConfiguration("QueueName", config, true);

        endpoint.Should().NotBeNull();
    }
}