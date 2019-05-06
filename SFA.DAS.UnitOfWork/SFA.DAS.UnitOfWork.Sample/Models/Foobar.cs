using System;
using SFA.DAS.UnitOfWork.Sample.Messages.Events;

namespace SFA.DAS.UnitOfWork.Sample.Models
{
    public class Foobar : Entity
    {
        public int Id { get; private set; }
        public DateTime Created { get; private set; }
        public DateTime? Updated { get; private set; }

        public Foobar(DateTime now)
        {
            Created = now;
            
            Publish(() => new FoobarCreatedEvent(Id, Created));
        }

        private Foobar()
        {
        }
        
        public void Update(DateTime now)
        {
            Updated = now;
        }
    }
}