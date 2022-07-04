using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.NetStandardMessages.Events;

namespace SFA.DAS.NServiceBus.NetCoreEndpoint
{
    internal static class Program
    {
        private const string AzureServiceBusConnectionString = "";

        public static async Task Main()
        {
            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.NServiceBus.NetCoreEndpoint")
                .UseErrorQueue("SFA.DAS.NServiceBus.NetCoreEndpoint-error")
                .UseInstallers()
                .UseMessageConventions()
                .UseNewtonsoftJsonSerializer()
            ;

            if (!string.IsNullOrEmpty(AzureServiceBusConnectionString))
            {
                endpointConfiguration
                    .UseAzureServiceBusTransport(AzureServiceBusConnectionString, _ => { });
            }
            else
            {
                const string learningTransportDirectory = "c://temp//.sfa.das.nservicebus.learning-transport";
                endpointConfiguration
                    .UseTransport<LearningTransport>()
                    .Transactions(TransportTransactionMode.ReceiveOnly)
                    .StorageDirectory(learningTransportDirectory);
            }

            var endpoint = await Endpoint.Start(endpointConfiguration);

            Console.WriteLine("*** SFA.DAS.NServiceBus.NetCoreEndpoint ***");
            Console.WriteLine("Press '1' to publish event");
            Console.WriteLine("Press any other key to exit");

            while (true)
            {
                var key = Console.ReadKey();
                
                Console.WriteLine();

                if (key.Key == ConsoleKey.D1)
                {
                    await endpoint.Publish(new NetCoreEvent(".NET Core"));
                    
                    Console.WriteLine("Published event...");
                }
                else
                {
                    break;
                }
            }
            
            await endpoint.Stop();
        }
    }
}