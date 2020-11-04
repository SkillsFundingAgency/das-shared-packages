using System;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.UniformSession;
using SFA.DAS.UnitOfWork.Context;
using SFA.DAS.UnitOfWork.Pipeline;

namespace SFA.DAS.UnitOfWork.NServiceBus.Pipeline
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly IUniformSession _uniformSession;
        private readonly IUnitOfWorkContext _unitOfWorkContext;

        public UnitOfWork(IUniformSession uniformSession, IUnitOfWorkContext unitOfWorkContext)
        {
            _uniformSession = uniformSession;
            _unitOfWorkContext = unitOfWorkContext;
        }

        public async Task CommitAsync(Func<Task> next)
        {
            var events = _unitOfWorkContext.GetEvents();

            await next().ConfigureAwait(false);
            await Task.WhenAll(events.Where(x => !(x is ICommand)).Select(_uniformSession.Publish)).ConfigureAwait(false);
            await Task.WhenAll(events.Where(x => x is ICommand).Select(_uniformSession.Send)).ConfigureAwait(false);
        }
    }
}