using System;

namespace SFA.DAS.NServiceBus
{
    public interface IOutboxTransaction : IDisposable
    {
        void Commit();
    }
}