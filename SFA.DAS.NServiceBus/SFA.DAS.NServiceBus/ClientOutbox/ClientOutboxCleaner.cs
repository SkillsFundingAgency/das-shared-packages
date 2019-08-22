using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Features;
using NServiceBus.Logging;
using NServiceBus.Settings;

namespace SFA.DAS.NServiceBus.ClientOutbox
{
    /// <summary>
    /// Based on the OutboxCleaner implementation in NServiceBus
    /// https://github.com/Particular/NServiceBus.Persistence.Sql/blob/4.1.1/src/SqlPersistence/Outbox/OutboxCleaner.cs
    /// </summary>
    public class ClientOutboxCleaner : FeatureStartupTask
    {
        private static readonly ILog Logger = LogManager.GetLogger<ClientOutboxCleaner>();
        
        private readonly IAsyncTimer _timer;
        private readonly IClientOutboxStorage _clientOutboxStorage;
        private readonly IClientOutboxStorageV2 _clientOutboxStorageV2;
        private readonly CriticalError _criticalError;
        private readonly TimeSpan _frequency;
        private readonly TimeSpan _maxAge;
        private int _failures;

        public ClientOutboxCleaner(IAsyncTimer timer, IClientOutboxStorage clientOutboxStorage, IClientOutboxStorageV2 clientOutboxStorageV2, ReadOnlySettings settings, CriticalError criticalError)
        {
            _timer = timer;
            _clientOutboxStorage = clientOutboxStorage;
            _clientOutboxStorageV2 = clientOutboxStorageV2;
            _criticalError = criticalError;
            _frequency = settings.GetOrDefault<TimeSpan?>("Persistence.Sql.Outbox.FrequencyToRunDeduplicationDataCleanup") ?? TimeSpan.FromMinutes(1);
            _maxAge = settings.GetOrDefault<TimeSpan?>("Persistence.Sql.Outbox.TimeToKeepDeduplicationData") ?? TimeSpan.FromDays(7);
        }

        protected override Task OnStart(IMessageSession messageSession)
        {            
            _timer.Start((d, c) => Cleanup(messageSession, d, c), OnError, _frequency);
            
            return Task.CompletedTask;
        }

        protected override Task OnStop(IMessageSession messageSession)
        {
            return _timer.Stop();
        }

        private async Task Cleanup(IMessageSession messageSession, DateTime now, CancellationToken cancellationToken)
        {
            var clientOutboxMessagesTask = _clientOutboxStorage.GetAwaitingDispatchAsync();
            var clientOutboxMessageV2sTask = _clientOutboxStorageV2.GetAwaitingDispatchAsync();
            var clientOutboxMessages = await clientOutboxMessagesTask.ConfigureAwait(false);
            var clientOutboxMessageV2s = await clientOutboxMessageV2sTask.ConfigureAwait(false);
            var oldest = now.Subtract(_maxAge);
            
            var tasks = clientOutboxMessages
                .Select(m =>
                {
                    var sendOptions = new SendOptions();

                    sendOptions.SetDestination(m.EndpointName);
                    sendOptions.SetMessageId(m.MessageId.ToString());

                    return messageSession.Send(new ProcessClientOutboxMessageCommand(), sendOptions);
                })
                .Concat(clientOutboxMessageV2s.Select(m =>
                {
                    var sendOptions = new SendOptions();

                    sendOptions.SetDestination(m.EndpointName);
                    sendOptions.SetMessageId(m.MessageId.ToString());
                        
                    return messageSession.Send(new ProcessClientOutboxMessageCommandV2(), sendOptions);
                }))
                .Concat(new []
                {
                    _clientOutboxStorage.RemoveEntriesOlderThanAsync(oldest, cancellationToken),
                    _clientOutboxStorageV2.RemoveEntriesOlderThanAsync(oldest, cancellationToken)
                });
                
            await Task.WhenAll(tasks).ConfigureAwait(false);
            
            _failures = 0;
        }

        private void OnError(Exception exception)
        {
            Logger.Error("Failed to cleanup client outbox messages", exception);
            
            _failures++;
            
            if (_failures >= 10)
            {
                _criticalError.Raise("Failed to cleanup client outbox messages after 10 consecutive unsuccessful attempts", exception);
                            
                _failures = 0;
            }
        }
    }
}