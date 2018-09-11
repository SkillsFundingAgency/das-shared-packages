using System;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus.UniformSession;

namespace SFA.DAS.UnitOfWork.NServiceBus
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IUniformSession _session;
        private readonly IUnitOfWorkContext _unitOfWorkContext;

        public UnitOfWork(IUniformSession session, IUnitOfWorkContext unitOfWorkContext)
        {
            _session = session;
            _unitOfWorkContext = unitOfWorkContext;
        }

        public async Task CommitAsync(Func<Task> next)
        {
            var events = _unitOfWorkContext.GetEvents();

            await next().ConfigureAwait(false);
            await Task.WhenAll(events.Select(_session.Publish)).ConfigureAwait(false);
        }
    }
}