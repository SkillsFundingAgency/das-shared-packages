using System.Threading.Tasks;

namespace SFA.DAS.Support.Shared
{
    public interface IClientValidatior
    {
        Task<bool> Validate(string token, string clientId, string appKey, string resourceId, string tenant);
    }
}