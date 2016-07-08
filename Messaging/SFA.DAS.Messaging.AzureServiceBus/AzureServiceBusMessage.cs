using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace SFA.DAS.Messaging.AzureServiceBus
{
    public class AzureServiceBusMessage<T> : Message<T>
    {
        private readonly BrokeredMessage _brokeredMessage;

        public AzureServiceBusMessage(BrokeredMessage brokeredMessage)
            : base(brokeredMessage.GetBody<T>())
        {
            _brokeredMessage = brokeredMessage;
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
