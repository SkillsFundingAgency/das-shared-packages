using System.Data.Entity;
using System.Threading.Tasks;

namespace SFA.DAS.NServiceBus.EntityFramework
{
    public interface IOutboxDbContext
    {
        DbSet<OutboxMessage> OutboxMessages { get; }

        Task<int> SaveChangesAsync();
    }
}