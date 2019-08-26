using System;
using System.Threading.Tasks;
using NServiceBus.Pipeline;
using SFA.DAS.UnitOfWork.NServiceBus.Pipeline;
using SFA.DAS.UnitOfWork.Pipeline;

namespace SFA.DAS.UnitOfWork.NServiceBus.Behaviors
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