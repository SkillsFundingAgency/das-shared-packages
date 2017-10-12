using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Messaging;
using SFA.DAS.Messaging.Interfaces;

namespace PubSubSampleGui.SubSystems
{
    public abstract class SubSystemBase
    {
        public event EventHandler<SampleMessageReceivedEventArgs> SampleMessageReceived;

        private IMessagePublisher _publisher;
        private IMessageSubscriberFactory _subscriberFactory;
        private Func<IEventingMessageReceiver<SampleMessage>> _eventingReceiverFactory;
        private CancellationTokenSource _cancellationSource;
        

        public virtual bool IsListening => _cancellationSource != null;

        public virtual async Task PublishAsync(SampleMessage message)
        {
            await _publisher.PublishAsync(message);
        }
        public virtual async Task<SampleMessage> PollAsync()
        {
            using (var subscriber = _subscriberFactory.GetSubscriber<SampleMessage>())
            {
                var message = await subscriber.ReceiveAsAsync();

                if (message == null)
                {
                    return null;
                }

                await message.CompleteAsync();

                return message.Content;
            }
        }
        public virtual void StartListening()
        {
            if (IsListening)
            {
                throw new InvalidOperationException("Already listening");
            }

            _cancellationSource = new CancellationTokenSource();
            var eventingReceiver = _eventingReceiverFactory.Invoke();
            eventingReceiver.MessageReceived += EventingReceiver_MessageReceived;
            eventingReceiver.Start(_cancellationSource.Token);
        }
        public virtual void StopListening()
        {
            if (!IsListening)
            {
                return;
            }

            _cancellationSource.Cancel();
        }

        protected void Init(
            IMessagePublisher publisher,
            IMessageSubscriberFactory subscriberFactory,
            Func<IEventingMessageReceiver<SampleMessage>> eventingReceiverFactory = null)
        {
            _publisher = publisher;
            _subscriberFactory = subscriberFactory;
            _eventingReceiverFactory = eventingReceiverFactory
                                       ?? DefaultEventingReceiverFactory;
        }
        protected void OnSampleMessageReceived(SampleMessage message)
        {
            SampleMessageReceived?.Invoke(this, new SampleMessageReceivedEventArgs { Message = message });
        }

        private void EventingReceiver_MessageReceived(object sender, MessageReceivedEventArgs<SampleMessage> e)
        {
            OnSampleMessageReceived(e.Message);
            e.Handled = true;
        }
        private IEventingMessageReceiver<SampleMessage> DefaultEventingReceiverFactory()
        {
            return new EventingMessageReceiverPollingWrapper<SampleMessage>(_subscriberFactory,
                new PollingToEventSettings
                {
                    PollingInterval = 2000
                });
        }
    }
}
