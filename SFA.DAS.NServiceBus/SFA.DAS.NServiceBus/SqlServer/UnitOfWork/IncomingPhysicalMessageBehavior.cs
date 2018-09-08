using System;
using System.Data.Common;
using System.Threading.Tasks;
using NServiceBus.Pipeline;
using SFA.DAS.NServiceBus.UnitOfWork;

namespace SFA.DAS.NServiceBus.SqlServer.UnitOfWork
{
    public class IncomingPhysicalMessageBehavior : Behavior<IIncomingPhysicalMessageContext>
    {
        public override async Task Invoke(IIncomingPhysicalMessageContext context, Func<Task> next)
        {
            var unitOfWorkContext = context.Builder.Build<IUnitOfWorkContext>();

            unitOfWorkContext.Set<DbConnection>(null);
            unitOfWorkContext.Set<DbTransaction>(null);

            await next().ConfigureAwait(false);
        }
    }
}