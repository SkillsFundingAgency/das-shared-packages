using System;
using System.Threading.Tasks;
using NServiceBus.Pipeline;
using SFA.DAS.UnitOfWork.Context;

namespace SFA.DAS.UnitOfWork.NServiceBus.Behaviors
{
    public class UnitOfWorkContextBehavior : Behavior<IInvokeHandlerContext>
    {
        public override async Task Invoke(IInvokeHandlerContext context, Func<Task> next)
        {
            var unitOfWorkContext = context.Builder.Build<IUnitOfWorkContext>();

            unitOfWorkContext.Set(context.SynchronizedStorageSession);

            await next().ConfigureAwait(false);
        }
    }
}