using System;
using System.Threading.Tasks;
using NServiceBus.Pipeline;

namespace SFA.DAS.UnitOfWork.NServiceBus
{
    public class UnitOfWorkBehavior : Behavior<IIncomingLogicalMessageContext>
    {
        public override async Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
        {
            var unitsOfWork = context.Builder.BuildAll<IUnitOfWork>();

            await next().ConfigureAwait(false);
            await unitsOfWork.ExceptClientUnitOfWork().CommitAsync().ConfigureAwait(false);
        }
    }
}