using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Messaging
{
    public abstract class MessageProcessor2<T> : IMessageProcessor2 where T: class, new()
    {
        // ReSharper disable once StaticMemberInGenericType
        private static readonly Task CompletedTask = Task.Delay(0);

        private readonly IMessageSubscriberFactory _messageSubscriberFactory;
        private readonly IMessageContextProvider _messageContextProvider;
        protected readonly ILog Log;

        protected MessageProcessor2(
            IMessageSubscriberFactory subscriberFactory, 
            ILog log,
            IMessageContextProvider messageContextProvider)
        {
            _messageSubscriberFactory = subscriberFactory;
            Log = log;
            _messageContextProvider = messageContextProvider;
        }

        public Task RunAsync(CancellationToken cancellationToken)
        {
            return Task.Run(() => Run(cancellationToken), cancellationToken);
        }

        public async Task Run(CancellationToken cancellationToken)
        {
            using (var subscriber = _messageSubscriberFactory.GetSubscriber<T>())
            {
                await ProcessMessagesUntilCancelled(subscriber, cancellationToken);
            }
        }

        private async Task ProcessMessagesUntilCancelled(IMessageSubscriber<T> subscriber, CancellationToken cancellationToken)
        {
            StartBackgroundCountLogger(cancellationToken);

            while (!cancellationToken.IsCancellationRequested)
            {
                var message = await GetMessage(subscriber);
                await ProcessMessage(message, cancellationToken);
            }
        }

        private async Task<IMessage<T>> GetMessage(IMessageSubscriber<T> subscriber)
        {
            IMessage<T> message;
            try
            {
                // This could throw an exception because the connection failed to open or a message could not be retrieved from the connection. We can't tell which 
                // so we'll assume it is because the connection failed to open and abandon.
                message = await subscriber.ReceiveAsAsync();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, $"Failed to retrieve message {typeof(T).FullName}");
                await OnFatalAsync(ex);
                throw;
            }

            return message;
        }

        private async Task ProcessMessage(IMessage<T> message, CancellationToken cancellationToken)
        {
            if (await IsNoMessage(message, cancellationToken))
            {
                return;
            }
    
            if (await IsNoMessageContent(message))
            { 
                return;
            }

            await HandleMessageWithContent(message);
        }

        private int _nullMessageCount = 0;
        private int _nullMessageContentCount = 0;

        private async Task<bool> IsNoMessage(IMessage<T> message, CancellationToken cancellationToken)
        {
            if (message == null)
            {
                Interlocked.Increment(ref _nullMessageCount);
                await Task.Delay(500, cancellationToken);
                return true;
            }

            return false;
        }

        private async Task<bool> IsNoMessageContent(IMessage<T> message)
        {
            if (message.Content == null)
            {
                Interlocked.Increment(ref _nullMessageContentCount);
                Log.Debug($"Message {message.Id} of type {typeof(T).FullName} has null content - ignoring message");
                await message.CompleteAsync();
                return true;
            }

            return false;
        }

        private void StartBackgroundCountLogger(CancellationToken cancellationToken)
        {
            Task.Run(() => LogNulls(cancellationToken), cancellationToken);
        }

        private void LogNulls(CancellationToken cancellationToken)
        {
            var logInterval = new TimeSpan(0, 0, 1, 0);

            while (!cancellationToken.IsCancellationRequested)
            {
                Task.Delay(logInterval, cancellationToken)
                    .ContinueWith(task =>
                    {
                        var nullMessageCount = Interlocked.Exchange(ref _nullMessageCount, 0);
                        var nullMessageContentCount = Interlocked.Exchange(ref _nullMessageContentCount, 0);

                        Log.Debug(
                            $"In the last {logInterval.TotalSeconds} seconds there have been {nullMessageCount} null messages of type {typeof(T).FullName} and {nullMessageContentCount} messages with null content");

                    }, cancellationToken);
            }
        }

        private async Task HandleMessageWithContent(IMessage<T> message)
        {
            try
            {
                Log.Debug($"Processing message of type {typeof(T).FullName}", message.Content.ToDictionary());
                try
                {
                    _messageContextProvider.StoreMessageContext(message);
                    await ProcessMessage(message.Content);
                }
                finally
                {
                    _messageContextProvider.ReleaseMessageContext(message);
                }

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
            return CompletedTask;
        }

        /// <summary>
        /// Override to get fatal error of this base class
        /// </summary>
        /// <param name="ex">Exception of the fatal error</param>
        /// <returns></returns>
        protected virtual Task OnFatalAsync(Exception ex)
        {
            //Do nothing unless someone overrides this method
            return CompletedTask;
        }
    }
}
