using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Messaging
{
    public abstract class MessageProcessor<T> : IMessageProcessor where T : new()
    {
        private readonly IMessageSubscriberFactory<T> _messageSubscriberFactory;
        protected readonly ILog Log;

        protected MessageProcessor(IMessageSubscriberFactory<T> subscriberFactory, ILog log)
        {
            _messageSubscriberFactory = subscriberFactory;
            Log = log;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            using (var subscriber = _messageSubscriberFactory.GetSubscriber())
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    var message = await subscriber.ReceiveAsAsync();
                    try
                    {
                        if (message == null)
                        {
                            continue;
                        }

                        if (message.Content == null)
                        {
                            await message.CompleteAsync();
                            continue;
                        }

                        await ProcessMessage(message.Content);

                        await message.CompleteAsync();
                        Log.Info($"Completed message {typeof(T).FullName}");
                        
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, $"Failed to process message {typeof(T).FullName}");

                        if (message != null && message.Content != null)
                        {
                            await message.AbortAsync();
                        }

                        await OnError(message);
                    }
                }
            }
        }

        protected abstract Task ProcessMessage(T messageContent);

        protected virtual async Task OnError(IMessage<T> message)
        {
            
        }
    }
}
