using System;
using NServiceBus.Raw;
using NServiceBus.Transport;

namespace SFA.DAS.NServiceBus.AzureFunction.Configuration
{
    public class NServiceBusOptions
    {
        public Func<RawEndpointConfiguration, RawEndpointConfiguration> EndpointConfiguration { get; set; }
        public Action<MessageContext> OnMessageReceived { get; set; }
        public Action<MessageContext> OnMessageProcessed { get; set; }
        public Action<Exception, MessageContext> OnMessageErrored { get; set; }
    }
}
