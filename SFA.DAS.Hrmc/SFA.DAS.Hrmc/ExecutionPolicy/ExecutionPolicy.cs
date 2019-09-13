using System;
using System.Threading.Tasks;
using Polly;

namespace SFA.DAS.Hrmc.ExecutionPolicy
{
    public abstract class ExecutionPolicy
    {
        protected Policy RootPolicy { get; set; }

        public virtual void Execute(Action action)
        {
            try
            {
                RootPolicy.Execute(action);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        public virtual async Task ExecuteAsync(Func<Task> action)
        {
            try
            {
                await RootPolicy.ExecuteAsync(action);
            }
            catch (Exception ex)
            {
                OnException(ex);
            }
        }

        public virtual T Execute<T>(Func<T> func)
        {
            try
            {
                return RootPolicy.Execute(func);
            }
            catch (Exception ex)
            {
                return OnException<T>(ex);
            }
        }

        public virtual async Task<T> ExecuteAsync<T>(Func<Task<T>> func)
        {
            try
            {
                return await RootPolicy.ExecuteAsync(func);
            }
            catch (Exception ex)
            {
                return OnException<T>(ex);
            }
        }


        protected virtual void OnException(Exception ex)
        {
            throw ex;
        }

        protected virtual T OnException<T>(Exception ex)
        {
            throw ex;
        }

        protected static Policy CreateRetryPolicy<T>(int numberOfRetries, TimeSpan waitBetweenTries, Action<Exception> onRetryableFailure = null)
            where T : Exception
        {
            var waits = new TimeSpan[numberOfRetries];
            for (var i = 0; i < waits.Length; i++) waits[i] = waitBetweenTries;

            return Policy.Handle<T>().WaitAndRetry(waits, (ex, wait) => { onRetryableFailure?.Invoke(ex); });
        }

        protected static Policy CreateAsyncRetryPolicy<T>(int numberOfRetries, TimeSpan waitBetweenTries, Action<Exception> onRetryableFailure = null)
            where T : Exception
        {
            var waits = new TimeSpan[numberOfRetries];
            for (var i = 0; i < waits.Length; i++) waits[i] = waitBetweenTries;

            return Policy.Handle<T>().WaitAndRetryAsync(waits, (ex, wait) => { onRetryableFailure?.Invoke(ex); });
        }

        protected static Policy CreateAsyncRetryPolicy<T>(Func<T, bool> canHandle, int numberOfRetries, TimeSpan waitBetweenTries, Action<Exception> onRetryableFailure = null)
            where T : Exception
        {
            var waits = new TimeSpan[numberOfRetries];
            for (var i = 0; i < waits.Length; i++) waits[i] = waitBetweenTries;

            return Policy.Handle(canHandle).WaitAndRetryAsync(waits, (ex, wait) => { onRetryableFailure?.Invoke(ex); });
        }
    }
}