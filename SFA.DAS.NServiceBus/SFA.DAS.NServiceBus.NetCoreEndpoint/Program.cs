using System;
using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.NServiceBus.AzureServiceBus;
using SFA.DAS.NServiceBus.NetStandardMessages;
using SFA.DAS.NServiceBus.NetStandardMessages.Events;
using SFA.DAS.NServiceBus.NewtonsoftJsonSerializer;

namespace SFA.DAS.NServiceBus.NetCoreEndpoint
{
    internal class Program
    {
        public static async Task Main()
        {
            var endpointConfiguration = new EndpointConfiguration("SFA.DAS.NServiceBus.NetCoreEndpoint")
                .UseAzureServiceBusTransport(false, () => Configuration.AzureServiceBusConnectionString, r => {})
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