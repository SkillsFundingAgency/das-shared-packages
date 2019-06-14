using Microsoft.Azure.WebJobs.Description;
using Microsoft.Azure.WebJobs.Host.Config;

namespace SFA.DAS.NServiceBus.AzureFunction.Infrastructure
{
    [Extension("NServiceBus")]
    public class NServiceBusExtensionConfig : IExtensionConfigProvider
    {
        public void Initialize(ExtensionConfigContext context)
        {
            context.AddBindingRule<NServiceBusTriggerAttribute>()
                .BindToTrigger(new NServiceBusTriggerBindingProvider());
        }
    }
}
