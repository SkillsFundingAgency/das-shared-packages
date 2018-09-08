using System.Threading.Tasks;

namespace SFA.DAS.NServiceBus.UnitOfWork
{
    public class NoOpDb : IDb
    {
        public Task SaveChangesAsync()
        {
            return Task.CompletedTask;
        }
    }
}