using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Primitives;
using FluentAssertions.Specialized;

namespace SFA.DAS.Testing
{
    public abstract class FluentTest<T> where T : class, new()
    {
        public void Run(Action<T> assert)
        {
            Run(null, null, assert);
        }

        public void Run(Action<T> act, Action<T> assert)
        {
            Run(null, act, assert);
        }

        public void Run(Action<T> arrange, Action<T> act, Action<T> assert)
        {
            var testFixture = new T();
            
            arrange?.Invoke(testFixture);
            act?.Invoke(testFixture);
            assert(testFixture);
        }

        public void Run(Action<T, object> assert)
        {
            Run(null, null, assert);
        }

        public void Run<TResult>(Func<T, TResult> act, Action<T, TResult> assert)
        {
            Run(null, act, assert);
        }

        public void Run<TResult>(Action<T> arrange, Func<T, TResult> act, Action<T, TResult> assert)
        {
            var testFixture = new T();

            arrange?.Invoke(testFixture);

            var actionResult = default(TResult);

            if (act != null)
            {
                actionResult = act(testFixture);
            }
            
            assert(testFixture, actionResult);
        }

        public void RunCheckException<TException>(Action<T> act, Func<T, Action, ExceptionAssertions<TException>> assert) where TException : Exception
        {
            RunCheckException(null, act, assert);
        }

        public void RunCheckException<TException>(Action<T> arrange, Action<T> act, Func<T, Action, ExceptionAssertions<TException>> assert) where TException : Exception
        {
            if (act == null) throw new ArgumentNullException(nameof(act));
            if (act == null) throw new ArgumentNullException(nameof(assert));

            var testFixture = new T();

            arrange?.Invoke(testFixture);
            assert(testFixture, () => act.Invoke(testFixture));
        }

        public void RunCheckException(Action<T> arrange, Action<T> act, Func<T, Action, AndConstraint<ObjectAssertions>> assert)
        {
            if (act == null) throw new ArgumentNullException(nameof(act));
            if (act == null) throw new ArgumentNullException(nameof(assert));

            var testFixture = new T();

            arrange?.Invoke(testFixture);
            assert(testFixture, () => act.Invoke(testFixture));
        }

        public Task RunAsync(Action<T> assert)
        {
            return RunAsync(null, null, assert);
        }

        public Task RunAsync(Func<T, Task> act, Action<T> assert)
        {
            return RunAsync(null, act, assert);
        }

        public async Task RunAsync(Action<T> arrange, Func<T, Task> act, Action<T> assert)
        {
            var testFixture = new T();

            arrange?.Invoke(testFixture);

            if (act != null)
            {
                await act(testFixture);
            }

            assert(testFixture);
        }

        public Task RunAsync(Action<T, object> assert)
        {
            return RunAsync(null, null, assert);
        }

        public Task RunAsync<TResult>(Func<T, Task<TResult>> act, Action<T, TResult> assert)
        {
            return RunAsync(null, act, assert);
        }

        public async Task RunAsync<TResult>(Action<T> arrange, Func<T, Task<TResult>> act, Action<T, TResult> assert)
        {
            var testFixture = new T();

            arrange?.Invoke(testFixture);

            var actionResult = default(TResult);

            if (act != null)
            {
                actionResult = await act(testFixture);
            }

            assert(testFixture, actionResult);
        }

        public Task RunAsyncCheckException(Func<T, Task> act,
            Action<T, Func<Task>> assert)
        {
            return RunAsyncCheckException(null, act, assert);
        }

        public Task RunAsyncCheckException(Action<T> arrange,
            Func<T, Task> act,
            Action<T, Func<Task>> assert)
        {

            if (act == null) throw new ArgumentNullException(nameof(act));
            if (act == null) throw new ArgumentNullException(nameof(assert));

            var testFixture = new T();

            arrange?.Invoke(testFixture);

            assert(testFixture, async () => await act(testFixture));

            return Task.CompletedTask;
        }

    }
}