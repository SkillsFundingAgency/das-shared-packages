using System;

namespace SFA.DAS.UnitOfWork.Sample.Messages.Events
{
    public class FoobarCreatedEvent
    {
        public int Id { get; }
        public DateTime Created { get; }

        public FoobarCreatedEvent(int id, DateTime created)
        {
            Id = id;
            Created = created;
        }
    }
}