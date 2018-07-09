using System;
using System.Threading.Tasks;

namespace SFA.DAS.NServiceBus
{
    public interface IOutboxTransaction : IDisposable
    {
        Task CommitAsync();
    }
}