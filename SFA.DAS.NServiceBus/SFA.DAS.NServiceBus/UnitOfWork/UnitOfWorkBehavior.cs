using System;
using System.Linq;
using System.Threading.Tasks;
using NServiceBus;
using NServiceBus.Pipeline;

namespace SFA.DAS.NServiceBus.UnitOfWork
{
    public class UnitOfWorkBehavior : Behavior<IIncomingLogicalMessageContext>
    {
        public override async Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
        {
            await next().ConfigureAwait(false);

            var db = context.Builder.Build<IDb>();
            var unitOfWorkContext = context.Builder.Build<IUnitOfWorkContext>();
            var events = unitOfWorkContext.GetEvents();

            await db.SaveChangesAsync().ConfigureAwait(false);
            await Task.WhenAll(events.Select(context.Publish)).ConfigureAwait(false);
        }
    }
}