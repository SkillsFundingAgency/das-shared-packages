using System.Data.Entity;

namespace SFA.DAS.NServiceBus.UnitTests.EntityFramework
{
    public class DbContextFake : DbContext
    {
        public virtual DbSet<OutboxMessage> OutboxMessages { get; set; }
    }
}