using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Azure.Identity;
using Microsoft.Azure.WebJobs.Host.Executors;
using Microsoft.Azure.WebJobs.Host.Listeners;
using NServiceBus;
using NServiceBus.Extensibility;
using NServiceBus.Raw;
using NServiceBus.Transport;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;
using SFA.DAS.NServiceBus.AzureFunction.Configuration;
using SFA.DAS.NServiceBus.Configuration.AzureServiceBus;

namespace SFA.DAS.NServiceBus.AzureFunction.Hosting
{
#pragma warning disable S3881 // "IDisposable" should be implemented correctly
    public class NServiceBusListener : IListener
#pragma warning restore S3881 // "IDisposable" should be implemented correctly
    {
        private readonly string _poisonMessageQueue;
        private const int ImmediateRetryCount = 3;

        private readonly ITriggeredFunctionExecutor _executor;
        private readonly NServiceBusTriggerAttribute _attribute;
        private readonly ParameterInfo _parameter;
        private readonly NServiceBusOptions _nServiceBusOptions;
        private IReceivingRawEndpoint _endpoint;
        private CancellationTokenSource _cancellationTokenSource;

        public NServiceBusListener(
            ITriggeredFunctionExecutor executor, 
            NServiceBusTriggerAttribute attribute, 
            ParameterInfo parameter,
            NServiceBusOptions nServiceBusOptions = null)
        {
            _executor = executor;
            _attribute = attribute;
            _parameter = parameter;
            _poisonMessageQueue = $"{attribute.Endpoint}-Error";
            _nServiceBusOptions = nServiceBusOptions ?? new NServiceBusOptions();
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            var endpointConfigurationRaw = RawEndpointConfiguration.Create(_attribute.Endpoint, OnMessage, _poisonMessageQueue);

            if (_nServiceBusOptions.EndpointConfiguration != null)
            {
                endpointConfigurationRaw = _nServiceBusOptions.EndpointConfiguration.Invoke(endpointConfigurationRaw);
            }
            else
            {
                if (IsLearningTransportEndpoint()) SetupLearningTransportEndpoint(endpointConfigurationRaw);
                else SetupAzureServiceBusTransportEndpoint(endpointConfigurationRaw);
            }

            if (!string.IsNullOrEmpty(EnvironmentVariables.NServiceBusLicense))
            {
                endpointConfigurationRaw.UseLicense(EnvironmentVariables.NServiceBusLicense);
            }

            endpointConfigurationRaw.DefaultErrorHandlingPolicy(_poisonMessageQueue, ImmediateRetryCount);
            endpointConfigurationRaw.AutoCreateQueue();

            _endpoint = await RawEndpoint.Start(endpointConfigurationRaw).ConfigureAwait(false);

            if (_nServiceBusOptions.OnStarted != null)
            {
                _nServiceBusOptions.OnStarted.Invoke(_endpoint);
            }

            await _endpoint.SubscriptionManager.Subscribe(_parameter.ParameterType, new ContextBag());
        }

        private bool IsLearningTransportEndpoint() => _attribute.Connection.Contains("LearningEndpoint");

        private void SetupAzureServiceBusTransportEndpoint(RawEndpointConfiguration endpointConfigurationRaw)
        {
            endpointConfigurationRaw.UseTransport<AzureServiceBusTransport>()
                .ConnectionString(_attribute.Connection)
                .Transactions(TransportTransactionMode.ReceiveOnly)
                .CustomTokenCredential(new DefaultAzureCredential())
                .SubscriptionRuleNamingConvention(RuleNameShortener.Shorten)
                ;
        }

        private void SetupLearningTransportEndpoint(RawEndpointConfiguration endpointConfigurationRaw)
        {
            if (string.IsNullOrEmpty(_attribute.LearningTransportStorageDirectory))
                throw new ArgumentException("LearningTransportStorageDirectory must be set");
            
            endpointConfigurationRaw.UseTransport<LearningTransport>()
                .Transactions(TransportTransactionMode.ReceiveOnly)
                .StorageDirectory(_attribute.LearningTransportStorageDirectory)
                ;
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

            if (_nServiceBusOptions.OnMessageReceived != null)
            {
                _nServiceBusOptions.OnMessageReceived.Invoke(context);
            }

            var result = await _executor.TryExecuteAsync(triggerData, _cancellationTokenSource.Token);

            if (_nServiceBusOptions.OnMessageProcessed != null)
            {
                _nServiceBusOptions.OnMessageProcessed.Invoke(context);
            }

            if (!result.Succeeded)
            {
                if (_nServiceBusOptions.OnMessageErrored != null)
                {
                    _nServiceBusOptions.OnMessageErrored.Invoke(result.Exception, context);
                }

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
            // Method intentionally left empty.
        }
    }
}
