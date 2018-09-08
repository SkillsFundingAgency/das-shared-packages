using System.Data.Entity;
using SFA.DAS.NServiceBus.ClientOutbox;

namespace SFA.DAS.NServiceBus.EntityFramework.UnitTests
{
    public class DbContextFake : DbContext
    {
        public virtual DbSet<ClientOutboxMessage> OutboxMessages { get; set; }
    }
}