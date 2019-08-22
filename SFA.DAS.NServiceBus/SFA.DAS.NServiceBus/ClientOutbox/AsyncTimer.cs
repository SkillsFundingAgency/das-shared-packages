using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.NServiceBus.ClientOutbox
{
    public class AsyncTimer : IAsyncTimer
    {
        private readonly Func<TimeSpan, CancellationToken, Task> _delay;
        private CancellationTokenSource _cancellationTokenSource;
        private Task _task;

        public AsyncTimer(Func<TimeSpan, CancellationToken, Task> delay)
        {
            _delay = delay;
        }
        
        public void Start(Func<DateTime, CancellationToken, Task> successCallback, Action<Exception> errorCallback, TimeSpan interval)
        {
            _cancellationTokenSource = new CancellationTokenSource();

            var cancellationToken = _cancellationTokenSource.Token;
            
            _task = Task.Run(async () =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        await _delay(interval, cancellationToken).ConfigureAwait(false);
                        await successCallback(DateTime.UtcNow, cancellationToken).ConfigureAwait(false);
                    }
                    catch (OperationCanceledException)
                    {
                    }
                    catch (Exception exception)
                    {
                        errorCallback(exception);
                    }
                }
            }, CancellationToken.None);
        }

        public Task Stop()
        {
            if (_cancellationTokenSource == null)
            {
                return Task.CompletedTask;
            }
            
            using (_cancellationTokenSource)
            {
                _cancellationTokenSource.Cancel();
            }
            
            return _task ?? Task.CompletedTask;
        }
    }
}