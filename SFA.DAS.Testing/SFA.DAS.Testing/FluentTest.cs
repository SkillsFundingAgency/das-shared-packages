using System;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Primitives;
using FluentAssertions.Specialized;

namespace SFA.DAS.Testing
{
    public abstract class FluentTest<T> where T : class, new()
    {
        public void Test(Action<T> assert)
        {
            Test(null, null, assert);
        }

        public void Test(Action<T> act, Action<T> assert)
        {
            Test(null, act, assert);
        }

        public void Test(Action<T> arrange, Action<T> act, Action<T> assert)
        {
            var testFixture = new T();
            
            arrange?.Invoke(testFixture);
            act?.Invoke(testFixture);
            assert(testFixture);
        }

        public void Test(Action<T, object> assert)
        {
            Test(null, null, assert);
        }

        public void Test<TResult>(Func<T, TResult> act, Action<T, TResult> assert)
        {
            Test(null, act, assert);
        }

        public void Test<TResult>(Action<T> arrange, Func<T, TResult> act, Action<T, TResult> assert)
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

        public void TestException<TException>(Action<T> act, Func<T, Action, ExceptionAssertions<TException>> assert) where TException : Exception
        {
            TestException(null, act, assert);
        }

        public void TestException<TException>(Action<T> arrange, Action<T> act, Func<T, Action, ExceptionAssertions<TException>> assert) where TException : Exception
        {
            if (act == null) throw new ArgumentNullException(nameof(act));
            if (assert == null) throw new ArgumentNullException(nameof(assert));

            var testFixture = new T();

            arrange?.Invoke(testFixture);
            assert(testFixture, () => act.Invoke(testFixture));
        }

        public void TestException(Action<T> arrange, Action<T> act, Func<T, Action, AndConstraint<ObjectAssertions>> assert)
        {
            if (act == null) throw new ArgumentNullException(nameof(act));
            if (assert == null) throw new ArgumentNullException(nameof(assert));

            var testFixture = new T();

            arrange?.Invoke(testFixture);
            assert(testFixture, () => act.Invoke(testFixture));
        }

        public Task TestAsync(Action<T> assert)
        {
            return TestAsync(null, null, assert);
        }

        public Task TestAsync(Func<T, Task> act, Action<T> assert)
        {
            return TestAsync(null, act, assert);
        }

        public async Task TestAsync(Action<T> arrange, Func<T, Task> act, Action<T> assert)
        {
            var testFixture = new T();

            arrange?.Invoke(testFixture);

            if (act != null)
            {
                await act(testFixture);
            }

            assert(testFixture);
        }

        public Task TestAsync(Action<T, object> assert)
        {
            return TestAsync(null, null, assert);
        }

        public Task TestAsync<TResult>(Func<T, Task<TResult>> act, Action<T, TResult> assert)
        {
            return TestAsync(null, act, assert);
        }

        public async Task TestAsync<TResult>(Action<T> arrange, Func<T, Task<TResult>> act, Action<T, TResult> assert)
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

        public Task TestExceptionAsync(Func<T, Task> act,
            Action<T, Func<Task>> assert)
        {
            return TestExceptionAsync(null, act, assert);
        }

        public Task TestExceptionAsync(Action<T> arrange,
            Func<T, Task> act,
            Action<T, Func<Task>> assert)
        {

            if (act == null) throw new ArgumentNullException(nameof(act));
            if (assert == null) throw new ArgumentNullException(nameof(assert));

            var testFixture = new T();

            arrange?.Invoke(testFixture);

            assert(testFixture, async () => await act(testFixture));

            return Task.CompletedTask;
        }

    }
}