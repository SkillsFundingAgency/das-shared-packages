using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace SFA.DAS.Messaging.AzureServiceBus
{
    public class AzureServiceBusMessage : SubSystemMessage
    {
        private readonly BrokeredMessage _brokeredMessage;

        public AzureServiceBusMessage(BrokeredMessage brokeredMessage)
        {
            _brokeredMessage = brokeredMessage;
            Content = brokeredMessage.GetBody<string>();
        }

        public override Task CompleteAsync()
        {
            return _brokeredMessage.CompleteAsync();
        }
        public override Task AbortAsync()
        {
            return _brokeredMessage.AbandonAsync();
        }
    }
}
