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
                    IMessage<T> message = null;

                    Log.Debug($"Getting message of type {typeof(T).FullName} from azure topic message queue");

                    try
                    {
                        message = await subscriber.ReceiveAsAsync();
                    }
                    catch (Exception ex)
                    {
                        Log.Fatal(ex, $"Failed to retrieve message {typeof(T).FullName}");
                        await OnFatalAsync(ex);

                        throw;
                    }

                    Log.Debug($"Recieved message of type {typeof(T).FullName} from azure topic message queue");

                    try
                    {
                        if (message == null)
                        {
                            Log.Debug($"No messages on queue of type {typeof(T).FullName}");
                            await Task.Delay(500, cancellationToken);
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

                        await OnErrorAsync(message, ex);
                    }
                }
            }
        }

        /// <summary>
        /// Implement to handle a message that needs to be processed
        /// </summary>
        /// <param name="messageContent"></param>
        /// <returns></returns>
        protected abstract Task ProcessMessage(T messageContent);

        /// <summary>
        /// Override to get error and message of this base class
        /// </summary>
        /// <param name="message">Message that has caused the error</param>
        /// <param name="ex">Exception that has occurred</param>
        /// <returns></returns>
        protected virtual Task OnErrorAsync(IMessage<T> message, Exception ex)
        {
            //Do nothing unless someone overrides this method
            return Task.Delay(0);
        }

        /// <summary>
        /// Override to get fatal error of this base class
        /// </summary>
        /// <param name="ex">Exception of the fatal error</param>
        /// <returns></returns>
        protected virtual Task OnFatalAsync(Exception ex)
        {
            //Do nothing unless someone overrides this method
            return Task.Delay(0);
        }
    }
}
