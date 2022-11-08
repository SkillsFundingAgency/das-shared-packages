using System.Net;
using Microsoft.Extensions.Configuration;
using NServiceBus;
using SFA.DAS.NServiceBus.Extensions;
using SFA.DAS.TrackProgress.Messages.Commands;
using SFA.DAS.TrackProgress.Messages.Events;

const string queueName = "SFA.DAS.ExtensionExample";

IConfiguration config = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddJsonFile("appsettings.development.json", optional: true)
    .Build();

// The connextion must be a real Azure SB endpoint
var connectionString = config["NServiceBusConnection"];
if (connectionString is null)
    throw new NotSupportedException("NServiceBusConnection should contain ServiceBus connection string");


var endpointConfiguration = new EndpointConfiguration(queueName);
endpointConfiguration.EnableInstallers();
endpointConfiguration.UseMessageConventions();
endpointConfiguration.UseNewtonsoftJsonSerializer();

endpointConfiguration.SendOnly();

var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
transport.AddRouting(routeSettings =>
{
    routeSettings.RouteToEndpoint(typeof(CacheKsbsCommand), queueName);
});

transport.ConnectionString(connectionString);

var endpointInstance = await Endpoint.Start(endpointConfiguration)
    .ConfigureAwait(false);

while (true)
{
    Console.Clear();
    Console.WriteLine("To Publish an Event please select the option...");
    Console.WriteLine("1. Publish NewProgressAddedEvent");
    Console.WriteLine("2. Send CachKsbsCommand");
    Console.WriteLine("X. Exit");

    var choice = Console.ReadLine()?.ToLower();
    switch (choice)
    {
        case "1":
            await PublishMessage(endpointInstance, new NewProgressAddedEvent { CommitmentsApprenticeshipId = 7887 });
            break;
        case "2":
            await SendMessage(endpointInstance, new CacheKsbsCommand { StandardUid = "CourseABC" });
            break;
        case "x":
            await endpointInstance.Stop();
            return;
    }
}

async Task PublishMessage(IMessageSession messageSession, object message)
{
    await messageSession.Publish(message);

    Console.WriteLine("Message published.");
    Console.WriteLine("Press enter to continue");
    Console.ReadLine();
}

async Task SendMessage(IMessageSession messageSession, object message)
{
    await messageSession.Send(message);

    Console.WriteLine("Message sent.");
    Console.WriteLine("Press enter to continue");
    Console.ReadLine();
}