using System;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using SFA.DAS.NServiceBus.Services;
using SFA.DAS.Testing;

namespace SFA.DAS.NServiceBus.UnitTests.Services
{
    [TestFixture]
    public class TimerServiceTests : FluentTest<TimerServiceTestsFixture>
    {
        [Test]
        public Task Start_WhenStarting_ThenShouldInvokeCallbackAfterDelay()
        {
            return TestAsync(
                f => f.SetCallbackSuccess(),
                f => f.Start(),
                f =>
                {
                    f.Delay.Verify(d => d(f.Interval, It.IsAny<CancellationToken>()), Times.AtLeastOnce);
                    f.SuccessCallback.Verify(c => c(f.Now, It.IsAny<CancellationToken>()), Times.AtLeastOnce);
                });
        }
        
        [Test]
        public Task Start_WhenExceptionIsThrown_ThenShouldInvokeErrorCallbackAfterDelay()
        {
            return TestAsync(
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
            return TestAsync(
                f => f.SetCallbackError(),
                f => f.Start(),
                f =>
                {
                    f.Delay.Verify(d => d(f.Interval, It.IsAny<CancellationToken>()), Times.AtLeast(2));
                    f.SuccessCallback.Verify(c => c(f.Now, It.IsAny<CancellationToken>()), Times.AtLeast(2));
                });
        }
        
        [Test]
        public Task Stop_WhenDelayed_ThenShouldStop()
        {
            return TestAsync(
                f => f.SetMaxInterval(),
                f => f.StartThenStopWhenDelayed(),
                f => f.TimerServiceCancellationToken.IsCancellationRequested.Should().BeTrue());
        }
        
        [Test]
        public Task Stop_WhenCallingBack_ThenShouldStop()
        {
            return TestAsync(
                f => f.SetCallbackSuccess(),
                f => f.StartThenStopWhenCallingBack(), 
                f => f.TimerServiceCancellationToken.IsCancellationRequested.Should().BeTrue());
        }

        [Test]
        public Task Stop_WhenNotStarted_ThenShouldNotThrowException()
        {
            return TestExceptionAsync(f => f.Stop(), (_, r) => r.Should().NotThrowAsync());
        }
    }

    public class TimerServiceTestsFixture
    {
        public TimeSpan TestTimeout { get; set; }
        public DateTime Now { get; set; }
        public Mock<IDateTimeService> DateTimeService { get; set; }
        public TaskCompletionSource<object> DelayTaskCompletionSource { get; set; }
        public CancellationTokenSource DelayCancellationTokenSource { get; set; }
        public Mock<Func<TimeSpan,CancellationToken,Task>> Delay { get; set; }
        public TaskCompletionSource<object> CallbackTaskCompletionSource { get; set; }
        public CancellationTokenSource CallbackCancellationTokenSource { get; set; }
        public Mock<Func<DateTime, CancellationToken, Task>> SuccessCallback { get; set; }
        public Mock<Action<Exception>> ErrorCallback { get; set; }
        public TimeSpan Interval { get; set; }
        public Exception Exception { get; set; }
        public CancellationToken TimerServiceCancellationToken { get; set; }
        public ITimerService TimerService { get; set; }

        public TimerServiceTestsFixture()
        {
            TestTimeout = TimeSpan.FromSeconds(10);
            Now = DateTime.UtcNow;
            DateTimeService = new Mock<IDateTimeService>();
            DelayTaskCompletionSource = new TaskCompletionSource<object>();
            DelayCancellationTokenSource = new CancellationTokenSource(TestTimeout);
            Delay = new Mock<Func<TimeSpan, CancellationToken, Task>>();
            CallbackTaskCompletionSource = new TaskCompletionSource<object>();
            CallbackCancellationTokenSource = new CancellationTokenSource(TestTimeout);
            SuccessCallback = new Mock<Func<DateTime, CancellationToken, Task>>();
            ErrorCallback = new Mock<Action<Exception>>();
            Interval = TimeSpan.Zero;
            Exception = new Exception();
            
            DateTimeService.Setup(d => d.UtcNow).Returns(Now);
            DelayCancellationTokenSource.Token.Register(() => DelayTaskCompletionSource.TrySetCanceled());
            CallbackCancellationTokenSource.Token.Register(() => CallbackTaskCompletionSource.TrySetCanceled());

            Delay.Setup(d => d(It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>())).Returns<TimeSpan, CancellationToken>((d, c) =>
            {
                TimerServiceCancellationToken = c;
                DelayTaskCompletionSource.TrySetResult(null);
                
                return Task.Delay(d, c);
            });
            
            TimerService = new TimerService(DateTimeService.Object, Delay.Object);
        }

        public Task Start()
        {
            TimerService.Start(SuccessCallback.Object, ErrorCallback.Object, Interval);
            return CallbackTaskCompletionSource.Task;
        }

        public async Task StartThenStopWhenDelayed()
        {
            TimerService.Start(SuccessCallback.Object, ErrorCallback.Object, Interval);
            await DelayTaskCompletionSource.Task;
            await TimerService.Stop();
        }

        public async Task StartThenStopWhenCallingBack()
        {
            TimerService.Start(SuccessCallback.Object, ErrorCallback.Object, Interval);
            await CallbackTaskCompletionSource.Task;
            await TimerService.Stop();
        }

        public Task Stop()
        {
            return TimerService.Stop();
        }

        public TimerServiceTestsFixture SetCallbackSuccess()
        {
            SuccessCallback.Setup(c => c(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .Returns<DateTime, CancellationToken>((_, _) =>
                {
                    CallbackTaskCompletionSource.TrySetResult(null);

                    return Task.CompletedTask;
                });
            
            return this;
        }

        public TimerServiceTestsFixture SetCallbackError()
        {
            var isSuccess = false;
            
            SuccessCallback.Setup(c => c(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .Returns<DateTime, CancellationToken>((_, c) =>
                {
                    TimerServiceCancellationToken = c;
                    
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

        public TimerServiceTestsFixture SetMaxInterval()
        {
            Interval = TimeSpan.FromMilliseconds(int.MaxValue);
            
            return this;
        }
    }
}