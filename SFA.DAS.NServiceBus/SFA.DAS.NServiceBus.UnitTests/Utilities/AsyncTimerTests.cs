using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.NServiceBus.Utilities;
using SFA.DAS.Testing;

namespace SFA.DAS.NServiceBus.UnitTests.Utilities
{
    [TestFixture]
    public class AsyncTimerTests : FluentTest<AsyncTimerTestsFixture>
    {
        [Test]
        public Task Start_WhenStarting_ThenShouldInvokeCallbackAfterDelay()
        {
            return RunAsync(
                f => f.SetCallbackSuccess(),
                f => f.Start(),
                f =>
                {
                    f.Delay.Verify(d => d(f.Interval, It.IsAny<CancellationToken>()), Times.AtLeastOnce);
                    f.SuccessCallback.Verify(c => c(It.Is<DateTime>(d => d >= f.Now), It.IsAny<CancellationToken>()), Times.AtLeastOnce);
                });
        }
        
        [Test]
        public Task Start_WhenExceptionIsThrown_ThenShouldInvokeErrorCallbackAfterDelay()
        {
            return RunAsync(
                f => f.SetCallbackError(),
                f => f.Start(), 
                f =>
                {
                    f.Delay.Verify(d => d(f.Interval, It.IsAny<CancellationToken>()), Times.AtLeastOnce);
                    f.ErrorCallback.Verify(c => c(f.Exception), Times.AtLeastOnce);
                });
        }
        
        [Test]
        public Task Start_WhenExceptionIsThrown_ThenShouldInvokeCallbackAfterDelays()
        {
            return RunAsync(
                f => f.SetCallbackError(),
                f => f.Start(),
                f =>
                {
                    f.Delay.Verify(d => d(f.Interval, It.IsAny<CancellationToken>()), Times.AtLeast(2));
                    f.SuccessCallback.Verify(c => c(It.Is<DateTime>(d => d >= f.Now), It.IsAny<CancellationToken>()), Times.AtLeast(2));
                });
        }
        
        [Test]
        public Task Stop_WhenDelayed_ThenShouldStop()
        {
            return RunAsync(
                f => f.SetMaxInterval(),
                f => f.StopWhenDelayed(),
                f => f.TimerCancellationToken.IsCancellationRequested.Should().BeTrue());
        }
        
        [Test]
        public Task Stop_WhenCallingBack_ThenShouldStop()
        {
            return RunAsync(
                f => f.SetCallbackSuccess(),
                f => f.StopWhenCallingBack(),
                f => f.TimerCancellationToken.IsCancellationRequested.Should().BeTrue());
        }
    }

    public class AsyncTimerTestsFixture
    {
        public TimeSpan TestTimeout { get; set; }
        public DateTime Now { get; set; }
        public TaskCompletionSource<object> DelayTaskCompletionSource { get; set; }
        public CancellationTokenSource DelayCancellationTokenSource { get; set; }
        public Mock<Func<TimeSpan,CancellationToken,Task>> Delay { get; set; }
        public TaskCompletionSource<object> CallbackTaskCompletionSource { get; set; }
        public CancellationTokenSource CallbackCancellationTokenSource { get; set; }
        public Mock<Func<DateTime, CancellationToken, Task>> SuccessCallback { get; set; }
        public Mock<Action<Exception>> ErrorCallback { get; set; }
        public TimeSpan Interval { get; set; }
        public Exception Exception { get; set; }
        public CancellationToken TimerCancellationToken { get; set; }
        public IAsyncTimer Timer { get; set; }

        public AsyncTimerTestsFixture()
        {
            TestTimeout = TimeSpan.FromSeconds(10);
            Now = DateTime.UtcNow;
            DelayTaskCompletionSource = new TaskCompletionSource<object>();
            DelayCancellationTokenSource = new CancellationTokenSource(TestTimeout);
            Delay = new Mock<Func<TimeSpan, CancellationToken, Task>>();
            CallbackTaskCompletionSource = new TaskCompletionSource<object>();
            CallbackCancellationTokenSource = new CancellationTokenSource(TestTimeout);
            SuccessCallback = new Mock<Func<DateTime, CancellationToken, Task>>();
            ErrorCallback = new Mock<Action<Exception>>();
            Interval = TimeSpan.Zero;
            Exception = new Exception();
            
            DelayCancellationTokenSource.Token.Register(() => DelayTaskCompletionSource.TrySetCanceled());
            CallbackCancellationTokenSource.Token.Register(() => CallbackTaskCompletionSource.TrySetCanceled());

            Delay.Setup(d => d(It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>())).Returns<TimeSpan, CancellationToken>((d, c) =>
            {
                TimerCancellationToken = c;
                DelayTaskCompletionSource.TrySetResult(null);
                
                return Task.Delay(d, c);
            });
            
            Timer = new AsyncTimer(Delay.Object);
        }

        public async Task Start()
        {
            Timer.Start(SuccessCallback.Object, ErrorCallback.Object, Interval);
            await CallbackTaskCompletionSource.Task;
        }

        public async Task StopWhenDelayed()
        {
            Timer.Start(SuccessCallback.Object, ErrorCallback.Object, Interval);
            await DelayTaskCompletionSource.Task;
            await Timer.Stop();
        }

        public async Task StopWhenCallingBack()
        {
            Timer.Start(SuccessCallback.Object, ErrorCallback.Object, Interval);
            await CallbackTaskCompletionSource.Task;
            await Timer.Stop();
        }

        public AsyncTimerTestsFixture SetCallbackSuccess()
        {
            SuccessCallback.Setup(c => c(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .Returns<DateTime, CancellationToken>((d, c) =>
                {
                    CallbackTaskCompletionSource.TrySetResult(null);

                    return Task.CompletedTask;
                });
            
            return this;
        }

        public AsyncTimerTestsFixture SetCallbackError()
        {
            var isSuccess = false;
            
            SuccessCallback.Setup(c => c(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .Returns<DateTime, CancellationToken>((d, c) =>
                {
                    TimerCancellationToken = c;
                    
                    if (!isSuccess)
                    {
                        isSuccess = true;
                        
                        throw Exception;
                    }
                    
                    CallbackTaskCompletionSource.TrySetResult(null);
                    
                    return Task.CompletedTask;
                });
            
            return this;
        }

        public AsyncTimerTestsFixture SetMaxInterval()
        {
            Interval = TimeSpan.FromMilliseconds(int.MaxValue);
            
            return this;
        }
    }
}