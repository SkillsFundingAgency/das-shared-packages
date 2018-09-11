using System;
using System.Threading.Tasks;
using NServiceBus.Pipeline;

namespace SFA.DAS.UnitOfWork.NServiceBus
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