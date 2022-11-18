using System.IO;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using SFA.DAS.NServiceBus.AzureFunction.Extensions;
using SFA.DAS.NServiceBus.AzureFunctions.Extensions.Example.Infrastructure;
using SFA.DAS.NServiceBus.Extensions;

[assembly: FunctionsStartup(typeof(SFA.DAS.NServiceBus.AzureFunctions.Extensions.Example.Startup))]
namespace SFA.DAS.NServiceBus.AzureFunctions.Extensions.Example;

public class Startup : FunctionsStartup
{
    public IConfiguration Configuration { get; set; }

    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
        builder.ConfigurationBuilder
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("local.settings.json", optional: true);
    }

    public override void Configure(IFunctionsHostBuilder builder)
    {
        Configuration = builder.GetContext().Configuration;

        builder.Services.AddLogging();

        InitialiseNServiceBus();

        builder.UseNServiceBus((IConfiguration appConfiguration) =>
        {
            var configuration =
                ServiceBusEndpointFactory.CreateSingleQueueConfiguration(QueueNames.ExtensionExample, appConfiguration,
                    false);
            configuration.AdvancedConfiguration.UseNewtonsoftJsonSerializer();
            configuration.AdvancedConfiguration.UseMessageConventions();
            configuration.AdvancedConfiguration.EnableInstallers();
            return configuration;
        });
    }

    public void InitialiseNServiceBus()
    {
        var m = new NServiceBusResourceManager(Configuration, false);
        m.CreateWorkAndErrorQueues(QueueNames.ExtensionExample).GetAwaiter().GetResult();
        m.SubscribeToTopicForQueue(typeof(Startup).Assembly, QueueNames.ExtensionExample).GetAwaiter().GetResult();
    }
}