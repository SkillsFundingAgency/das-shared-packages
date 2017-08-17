using System;
using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Messaging
{
    public abstract class MessageProcessor<T> : IMessageProcessor where T : new()
    {
        private readonly IPollingMessageReceiver _pollingMessageReceiver;
        protected readonly ILog Log;

        protected MessageProcessor(IPollingMessageReceiver pollingMessageReceiver, ILog log)
        {
            _pollingMessageReceiver = pollingMessageReceiver;
            Log = log;
        }

        public async Task RunAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                var message = await _pollingMessageReceiver.ReceiveAsAsync<T>();
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
                    await OnError(message);
                }

            }
        }

        protected abstract Task ProcessMessage(T messageContent);

        protected virtual async Task OnError(IMessage<T> message)
        {
            
        }
    }
}
