using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.TestMessages.Events;

namespace SFA.DAS.NServiceBus.TestEndpoint
{
    internal static class Program
    {
        private const string AzureServiceBusConnectionString = "";

        public static async Task Main()
        {
            var endpointConfiguration = new EndpointConfiguration(TestHarnessSettings.SampleQueueName)
                .UseErrorQueue(TestHarnessSettings.SampleQueueName + "-error")
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
                endpointConfiguration
                    .UseTransport<LearningTransport>()
                    .Transactions(TransportTransactionMode.ReceiveOnly)
                    .StorageDirectory(TestHarnessSettings.LearningTransportDirectory);
            }

            var endpoint = await Endpoint.Start(endpointConfiguration);

            Console.WriteLine("*** SFA.DAS.NServiceBus.TestEndpoint ***");
            Console.WriteLine("Press '1' to publish event");
            Console.WriteLine("Press any other key to exit");

            while (true)
            {
                var key = Console.ReadKey();
                
                Console.WriteLine();

                if (key.Key == ConsoleKey.D1)
                {
                    await endpoint.Publish(new SampleEvent("Hello world!"));
                    
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