using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.NServiceBus.AzureFunction.Hosting;
using SFA.DAS.NServiceBus.AzureFunctionExample.Extensions;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((hostBuilderContext, builder) => { builder.BuildDasConfiguration(hostBuilderContext.Configuration); })
    .ConfigureServices((context, s) =>
    {
        var serviceProvider = s.BuildServiceProvider();
        var configuration = context.Configuration;

        var configBuilder = new ConfigurationBuilder()
        .AddConfiguration(configuration)
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddEnvironmentVariables();

        var config = configBuilder.Build();

        s.AddOptions();
    })
    .Build();

host.Run();
