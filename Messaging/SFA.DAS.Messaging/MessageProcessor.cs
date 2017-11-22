using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Messaging
{
    public abstract class MessageProcessor<T> : IMessageProcessor where T : new()
    {
        private readonly IMessageSubscriberFactory _messageSubscriberFactory;
        protected readonly ILog Log;

        protected MessageProcessor(IMessageSubscriberFactory subscriberFactory, ILog log)
        {
            _messageSubscriberFactory = subscriberFactory;
            Log = log;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            using (var subscriber = _messageSubscriberFactory.GetSubscriber<T>())
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    Log.Debug($"Getting message of type {typeof(T).FullName} from azure topic message queue");

                    var message = await subscriber.ReceiveAsAsync();

                    Log.Debug($"Recieved message of type {typeof(T).FullName} from azure topic message queue");

                    try
                    {
                        if (message == null)
                        {
                            Log.Debug($"Message of type {typeof(T).FullName} is null");
                            continue;
                        }

                        if (message.Content == null)
                        {
                            Log.Debug($"Message of type {typeof(T).FullName} has null content");

                            await message.CompleteAsync();
                            continue;
                        }

                        Log.Debug($"Processing message of type {typeof(T).FullName}");
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
