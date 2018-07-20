using System.Data.Entity;

namespace SFA.DAS.NServiceBus.EntityFramework.UnitTests
{
    public class DbContextFake : DbContext
    {
        public virtual DbSet<OutboxMessage> OutboxMessages { get; set; }
    }
}