using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using SFA.DAS.NServiceBus.AzureFunction.Extensions;
using SFA.DAS.NServiceBus.AzureFunctions.Extensions.Example.Infrastructure;
using SFA.DAS.NServiceBus.AzureFunctions.Extensions.Example.StartUpExtensions;
using SFA.DAS.NServiceBus.Extensions;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((hostBuilderContext, builder) => { builder.BuildDasConfiguration(); })
    .ConfigureServices((context, s) =>
    {
        var configuration = context.Configuration;

        s.AddLogging();

        // Initialise NServiceBus
        var m = new NServiceBusResourceManager(configuration, false);
        m.CreateWorkAndErrorQueues(QueueNames.ExtensionExample).GetAwaiter().GetResult();
        m.SubscribeToTopicForQueue(typeof(Program).Assembly, QueueNames.ExtensionExample).GetAwaiter().GetResult();

        // Initialise endpoint and register it
        var endpointConfiguration =
                ServiceBusEndpointFactory.CreateSingleQueueConfiguration(QueueNames.ExtensionExample, configuration,
                    false);
        endpointConfiguration.UseNewtonsoftJsonSerializer();
        endpointConfiguration.UseMessageConventions();
        endpointConfiguration.EnableInstallers();
        var endpointInstance = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
        s.AddSingleton(endpointInstance);

    })
    .Build();

host.Run();
