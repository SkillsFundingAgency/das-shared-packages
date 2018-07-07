using System.Threading.Tasks;

namespace SFA.DAS.NServiceBus
{
    public interface IDb
    {
        Task SaveChangesAsync();
    }
}