using System;
using System.Threading.Tasks;
using NServiceBus.Persistence;

namespace SFA.DAS.NServiceBus.Features.ClientOutbox.Data
{
    public interface IClientOutboxTransaction : IDisposable, SynchronizedStorageSession
    {
        Task CommitAsync();
    }
}