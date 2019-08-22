using System;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.NServiceBus.ClientOutbox
{
    public interface IAsyncTimer
    {
        void Start(Func<DateTime, CancellationToken, Task> successCallback, Action<Exception> errorCallback, TimeSpan interval);
        Task Stop();
    }
}