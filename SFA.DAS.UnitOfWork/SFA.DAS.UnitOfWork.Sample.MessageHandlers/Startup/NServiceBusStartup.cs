using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NServiceBus;
using NServiceBus.ObjectBuilder.MSDependencyInjection;
using SFA.DAS.NServiceBus;
using SFA.DAS.NServiceBus.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.SqlServer;
using SFA.DAS.UnitOfWork.NServiceBus;

namespace SFA.DAS.UnitOfWork.Sample.MessageHandlers.Startup
{
    public static class NServiceBusStartup
    {
        public static void StartNServiceBus(this UpdateableServiceProvider serviceProvider, IConfiguration configuration)
        {
            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.UnitOfWork.Sample.MessageHandlers")
                .UseLearningTransport()
                .UseErrorQueue()
                .UseInstallers()
                .UseMessageConventions()
                .UseNewtonsoftJsonSerializer()
                .UseOutbox()
                .UseServicesBuilder(serviceProvider)
                .UseSqlServerPersistence(() => new SqlConnection(configuration.GetConnectionString("SampleDb")))
                .UseUnitOfWork();
            
            var endpoint = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
            
            serviceProvider.AddSingleton(p => endpoint)
                .AddHostedService<NServiceBusHostedService>();
        }
    }
}