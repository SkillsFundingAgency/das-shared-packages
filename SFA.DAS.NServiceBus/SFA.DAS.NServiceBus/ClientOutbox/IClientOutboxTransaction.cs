using System;
using System.Threading.Tasks;
using NServiceBus.Persistence;

namespace SFA.DAS.NServiceBus.ClientOutbox
{
    public interface IClientOutboxTransaction : IDisposable, SynchronizedStorageSession
    {
        Task CommitAsync();
    }
}