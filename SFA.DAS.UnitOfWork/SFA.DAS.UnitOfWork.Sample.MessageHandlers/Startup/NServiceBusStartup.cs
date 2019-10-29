using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.MicrosoftDependencyInjection;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.Hosting;
using SFA.DAS.NServiceBus.SqlServer.Configuration;
using SFA.DAS.UnitOfWork.NServiceBus;
using SFA.DAS.UnitOfWork.NServiceBus.Configuration;

namespace SFA.DAS.UnitOfWork.Sample.MessageHandlers.Startup
{
    public static class NServiceBusStartup
    {
        public static void StartNServiceBus(this UpdateableServiceProvider serviceProvider, IConfiguration configuration)
        {
            var endpointName = "SFA.DAS.UnitOfWork.Sample.MessageHandlers";
            var endpointConfiguration = new EndpointConfiguration(endpointName)
                .UseLearningTransport()
                .UseErrorQueue($"{endpointName}-errors")
                .UseInstallers()
                .UseMessageConventions()
                .UseNewtonsoftJsonSerializer()
                .UseOutbox()
                .UseServicesBuilder(serviceProvider)
                .UseSqlServerPersistence(() => new SqlConnection(configuration.GetConnectionString("SampleDb")))
                .UseUnitOfWork();
            
            var endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
            
            serviceProvider.AddSingleton(p => endpoint)
                .AddSingleton<IMessageSession>(p => p.GetService<IEndpointInstance>())
                .AddHostedService<NServiceBusHostedService>();
        }
    }
}