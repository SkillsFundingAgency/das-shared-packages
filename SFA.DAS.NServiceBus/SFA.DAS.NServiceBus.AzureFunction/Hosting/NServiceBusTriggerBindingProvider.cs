using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host.Triggers;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;
using SFA.DAS.NServiceBus.AzureFunction.Configuration;

namespace SFA.DAS.NServiceBus.AzureFunction.Hosting
{
    public class NServiceBusTriggerBindingProvider : ITriggerBindingProvider
    {
        private readonly NServiceBusOptions _nServiceBusOptions;

        public NServiceBusTriggerBindingProvider(NServiceBusOptions nServiceBusOptions = null)
        {
            _nServiceBusOptions = nServiceBusOptions ?? new NServiceBusOptions();
        }

        public Task<ITriggerBinding> TryCreateAsync(TriggerBindingProviderContext context)
        {
            var parameter = context.Parameter;
            var attribute = parameter.GetCustomAttribute<NServiceBusTriggerAttribute>(false);

            if (attribute == null)
            {
                return Task.FromResult<ITriggerBinding>(null);
            }

            if (string.IsNullOrEmpty(attribute.Connection))
            {
#if NET6_0
                attribute.Connection = EnvironmentVariables.NServiceBusConnectionString?.Replace("Endpoint=sb://", "");
#else
                attribute.Connection = EnvironmentVariables.NServiceBusConnectionString;
#endif
            }

            if (string.IsNullOrEmpty(attribute.LearningTransportStorageDirectory))
            {
                attribute.LearningTransportStorageDirectory = EnvironmentVariables.LearningTransportStorageDirectory;
            }

            return Task.FromResult<ITriggerBinding>(new NServiceBusTriggerBinding(parameter, attribute, _nServiceBusOptions));
        }
    }
}
