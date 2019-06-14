using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using NServiceBus;
using NServiceBus.Extensibility;
using NServiceBus.Raw;
using NServiceBus.Transport;
using SFA.DAS.NServiceBus.AzureServiceBus;

namespace SFA.DAS.NServiceBus.AzureFunction.Infrastructure
{
    public class NServiceBusListener : IListener
    {
        private const string PoisonMessageQueue = "error";
        private const int ImmediateRetryCount = 3;

        private readonly ITriggeredFunctionExecutor _executor;
        private readonly NServiceBusTriggerAttribute _attribute;
        private readonly ParameterInfo _parameter;
        private IReceivingRawEndpoint _endpoint;
        private CancellationTokenSource _cancellationTokenSource;

        public NServiceBusListener(ITriggeredFunctionExecutor executor, NServiceBusTriggerAttribute attribute,ParameterInfo parameter)
        {
            _executor = executor;
            _attribute = attribute;
            _parameter = parameter;
        }
      
        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var nameShortener = new RuleNameShortener();
            var endpointConfigurationRaw = RawEndpointConfiguration.Create(_attribute.EndPoint, OnMessage, PoisonMessageQueue);

            endpointConfigurationRaw.UseTransport<AzureServiceBusTransport>().RuleNameShortener(nameShortener.Shorten)
                
                .ConnectionString(_attribute.Connection)
                .Transactions(TransportTransactionMode.ReceiveOnly);

            if (!string.IsNullOrEmpty(EnvironmentVariables.NServiceBusLicense))
            {
                endpointConfigurationRaw.License(EnvironmentVariables.NServiceBusLicense);
            }
            endpointConfigurationRaw.DefaultErrorHandlingPolicy(PoisonMessageQueue, ImmediateRetryCount);
            endpointConfigurationRaw.AutoCreateQueue();

            _endpoint = await RawEndpoint.Start(endpointConfigurationRaw).ConfigureAwait(false);
            
            await _endpoint.SubscriptionManager.Subscribe(_parameter.ParameterType, new ContextBag());
            

        }
        
        protected async Task OnMessage(MessageContext context, IDispatchMessages dispatcher)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            var triggerData = new TriggeredFunctionData
            {
                TriggerValue = new NServiceBusTriggerData
                {
                    Data = context.Body,
                    Headers = context.Headers,
                    Dispatcher = dispatcher
                }
            };

            var result = await _executor.TryExecuteAsync(triggerData, _cancellationTokenSource.Token);

            if (!result.Succeeded)
            {
                throw result.Exception;
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Cancel();
            return _endpoint.Stop();
        }

        public void Cancel()
        {
            _cancellationTokenSource?.Cancel();
        }

        public void Dispose()
        {
        }
    }
}
