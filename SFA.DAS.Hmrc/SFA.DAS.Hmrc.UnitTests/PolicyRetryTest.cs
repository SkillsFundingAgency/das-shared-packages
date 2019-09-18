using System;
using System.Threading.Tasks;
using HMRC.ESFA.Levy.Api.Types.Exceptions;
using Moq;
using NUnit.Framework;
using SFA.DAS.Hmrc.ExecutionPolicy;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.Hmrc.UnitTests
{
    [TestFixture]
    public class PolicyRetryTest
    {
        private HmrcExecutionPolicy _policy;

        [SetUp]
        public void Init()
        {
            _policy = new HmrcExecutionPolicy(Mock.Of<ILog>(), TimeSpan.FromMilliseconds(0));
        }

        [Test]
        public void LimitedRetryWhenServiceUnavailableReturned()
        {
            var runner = new TestRunner<ApiHttpException>(CreateTestException(503, "service unavailable"));

            Assert.ThrowsAsync<ApiHttpException>(() => _policy.ExecuteAsync(runner.Execute));

            Assert.AreEqual(6, runner.CallCount);
        }

        [Test]
        public void LimitedRetryWhenInternalServerErrorReturned()
        {
            var runner = new TestRunner<ApiHttpException>(CreateTestException(500, "internal server error"));

            Assert.ThrowsAsync<ApiHttpException>(() => _policy.ExecuteAsync(runner.Execute));

            Assert.AreEqual(6, runner.CallCount);
        }

        [Test]
        public async Task RetryForeverWhenTooManyRequestsReturned()
        {
            var runner = new TestRunner<ApiHttpException>(CreateTestException(429, "too many requests"));

            await _policy.ExecuteAsync(runner.Execute);

            Assert.AreEqual(runner.MaxCallCount, runner.CallCount);
        }

        [Test]
        public async Task RetryForeverWhenRequestTimeoutReturned()
        {
            var runner = new TestRunner<ApiHttpException>(CreateTestException(408, "request timeout"));

            await _policy.ExecuteAsync(runner.Execute);

            Assert.AreEqual(runner.MaxCallCount, runner.CallCount);
        }

        private static ApiHttpException CreateTestException(int httpCode, string message)
        {
            return new ApiHttpException(httpCode, message, string.Empty, string.Empty);
        }

        private class TestRunner<T> where T : Exception
        {
            private readonly T _exception;

            public int CallCount { get; private set; }
            public int MaxCallCount { get; }

            public TestRunner(T exception, int maxCallCount = 10)
            {
                _exception = exception;
                MaxCallCount = maxCallCount;
                CallCount = 0;
            }

            public Task Execute()
            {
                if (++CallCount >= MaxCallCount)
                    return Task.CompletedTask;

                throw _exception;
            }
        }
    }
}
