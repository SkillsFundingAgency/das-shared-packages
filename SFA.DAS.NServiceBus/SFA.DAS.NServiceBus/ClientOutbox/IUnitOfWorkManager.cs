using System;

namespace SFA.DAS.NServiceBus.ClientOutbox
{
    public interface IUnitOfWorkManager
    {
        void Begin();
        void End(Exception ex = null);
    }
}