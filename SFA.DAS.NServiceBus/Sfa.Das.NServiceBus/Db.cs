using System.Threading.Tasks;

namespace SFA.DAS.NServiceBus
{
    public class Db : IDb
    {
        public Task SaveChangesAsync()
        {
            return Task.CompletedTask;
        }
    }
}