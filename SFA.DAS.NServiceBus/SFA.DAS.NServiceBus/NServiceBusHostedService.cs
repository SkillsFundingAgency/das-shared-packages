using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NServiceBus;

namespace SFA.DAS.NServiceBus
{
    public class NServiceBusHostedService : IHostedService
    {
        private readonly IEndpointInstance _endpoint;

        public NServiceBusHostedService(IEndpointInstance endpoint)
        {
            _endpoint = endpoint;
        }
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return _endpoint.Stop();
        }
    }
}