using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.Messaging
{
    public class EventingMessageReceiverPollingWrapper<T> : IEventingMessageReceiver<T> where T: new()
    {
        public event EventHandler<MessageReceivedEventArgs<T>> MessageReceived;

        private readonly IPollingMessageReceiver _pollingReceiver;
        private readonly PollingToEventSettings _settings;

        public EventingMessageReceiverPollingWrapper(IPollingMessageReceiver pollingReceiver, PollingToEventSettings settings)
        {
            _pollingReceiver = pollingReceiver;
            _settings = settings;
        }

        public void Start(CancellationToken cancellationToken)
        {
            RunAsync(cancellationToken);
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            if (MessageReceived == null)
            {
                throw new InvalidOperationException("No event handler for MessageReceived");
            }

            await Task.Factory.StartNew(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var message = await _pollingReceiver.ReceiveAsAsync<T>();
                    if (message != null)
                    {
                        try
                        {
                            var eventArgs = new MessageReceivedEventArgs<T>(message.Content);
                            MessageReceived(this, eventArgs);
                            if (eventArgs.Handled)
                            {
                                await message.CompleteAsync();
                            }
                            else
                            {
                                await message.AbortAsync();
                            }
                        }
                        catch
                        {
                            await message.AbortAsync();
                            throw;
                        }
                    }
                    Thread.Sleep(_settings.PollingInterval);
                }
            }, cancellationToken);
        }
    }
}
