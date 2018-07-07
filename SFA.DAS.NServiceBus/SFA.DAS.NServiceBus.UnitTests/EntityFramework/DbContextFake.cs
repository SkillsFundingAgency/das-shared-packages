using System.Data.Entity;
using SFA.DAS.NServiceBus.EntityFramework;

namespace SFA.DAS.NServiceBus.UnitTests.EntityFramework
{
    public class DbContextFake : DbContext, IOutboxDbContext
    {
        public virtual DbSet<OutboxMessage> OutboxMessages { get; set; }
    }
}