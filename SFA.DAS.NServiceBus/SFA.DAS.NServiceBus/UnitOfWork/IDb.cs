using System.Threading.Tasks;

namespace SFA.DAS.NServiceBus.UnitOfWork
{
    public interface IDb
    {
        Task SaveChangesAsync();
    }
}