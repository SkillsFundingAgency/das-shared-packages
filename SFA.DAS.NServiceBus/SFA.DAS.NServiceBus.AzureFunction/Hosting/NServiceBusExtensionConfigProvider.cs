using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;
using SFA.DAS.NServiceBus.AzureFunction.Configuration;

namespace SFA.DAS.NServiceBus.AzureFunction.Hosting
{
    [Extension("NServiceBus")]
    public class NServiceBusExtensionConfigProvider : IExtensionConfigProvider
    {
        private readonly NServiceBusOptions _nServiceBusOptions;

        public NServiceBusExtensionConfigProvider(NServiceBusOptions nServiceBusOptions = null)
        {
            _nServiceBusOptions = nServiceBusOptions ?? new NServiceBusOptions();
        }

        public void Initialize(ExtensionConfigContext context)
        {
            context.AddBindingRule<NServiceBusTriggerAttribute>()
                .BindToTrigger(new NServiceBusTriggerBindingProvider(_nServiceBusOptions));
        }
    }
}