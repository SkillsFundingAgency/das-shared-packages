using System;
using System.Threading.Tasks;

namespace SFA.DAS.NServiceBus.ClientOutbox
{
    public interface IClientOutboxTransaction : IDisposable
    {
        Task CommitAsync();
    }
}