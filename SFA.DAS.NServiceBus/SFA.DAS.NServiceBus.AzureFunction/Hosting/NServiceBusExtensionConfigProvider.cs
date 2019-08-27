using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;
using SFA.DAS.NServiceBus.AzureFunction.Attributes;

namespace SFA.DAS.NServiceBus.AzureFunction.Hosting
{
    [Extension("NServiceBus")]
    public class NServiceBusExtensionConfigProvider : IExtensionConfigProvider
    {
        public void Initialize(ExtensionConfigContext context)
        {
            context.AddBindingRule<NServiceBusTriggerAttribute>()
                .BindToTrigger(new NServiceBusTriggerBindingProvider());
        }
    }
}