using System.Threading.Tasks;

namespace SFA.DAS.Http.TokenGenerators
{
    public interface IGenerateBearerToken
    {
        Task<string> Generate();
    }
}
