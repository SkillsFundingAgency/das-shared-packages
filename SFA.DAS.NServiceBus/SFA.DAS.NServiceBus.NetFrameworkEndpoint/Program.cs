using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.NServiceBus.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;
using SFA.DAS.NServiceBus.Configuration.NewtonsoftJsonSerializer;
using SFA.DAS.NServiceBus.NetStandardMessages.Events;

namespace SFA.DAS.NServiceBus.NetFrameworkEndpoint
{
    internal class Program
    {
        private const string AzureServiceBusConnectionString = "";
        
        public static async Task Main()
        {
            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.NServiceBus.NetFrameworkEndpoint")
                .UseAzureServiceBusTransport(AzureServiceBusConnectionString, r => {})
                .UseErrorQueue()
                .UseInstallers()
                .UseMessageConventions()
                .UseNewtonsoftJsonSerializer();
            
            var endpoint = await Endpoint.Start(endpointConfiguration);

            Console.WriteLine("Press '1' to publish event");
            Console.WriteLine("Press any other key to exit");

            while (true)
            {
                var key = Console.ReadKey();
                
                Console.WriteLine();

                if (key.Key == ConsoleKey.D1)
                {
                    await endpoint.Publish(new NetFrameworkEvent(".NET Framework"));
                    
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