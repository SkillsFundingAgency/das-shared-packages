using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.NServiceBus.Services
{
    public interface ITimerService
    {
        void Start(Func<DateTime, CancellationToken, Task> successCallback, Action<Exception> errorCallback, TimeSpan interval);
        Task Stop();
    }
}