using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NServiceBus;
using SFA.DAS.UnitOfWork.Sample.Data;
using SFA.DAS.UnitOfWork.Sample.Messages.Events;

namespace SFA.DAS.UnitOfWork.Sample.MessageHandlers.EventHandlers
{
    public class FoobarCreatedEventHandler : IHandleMessages<FoobarCreatedEvent>
    {
        private readonly Lazy<SampleDbContext> _db;

        public FoobarCreatedEventHandler(Lazy<SampleDbContext> db)
        {
            _db = db;
        }

        public async Task Handle(FoobarCreatedEvent message, IMessageHandlerContext context)
        {
            var foobar = await _db.Value.Foobars.SingleAsync(f => f.Id == message.Id);
            
            foobar.Update(DateTime.UtcNow);
        }
    }
}