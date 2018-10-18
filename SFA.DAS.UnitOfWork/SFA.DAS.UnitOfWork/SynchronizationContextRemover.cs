using System;
using System.Threading;

namespace SFA.DAS.UnitOfWork
{
    public class SynchronizationContextRemover : IDisposable
    {
        private readonly SynchronizationContext _synchronizationContext;
        
        public SynchronizationContextRemover()
        {
            _synchronizationContext = SynchronizationContext.Current;
            
            SynchronizationContext.SetSynchronizationContext(null);
        }

        public void Dispose()
        {
            SynchronizationContext.SetSynchronizationContext(_synchronizationContext);
        }
    }
}