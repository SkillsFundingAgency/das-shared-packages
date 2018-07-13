using System;

namespace SFA.DAS.NServiceBus
{
    public interface IUnitOfWorkManager
    {
        void Begin();
        void End(Exception ex = null);
    }
}