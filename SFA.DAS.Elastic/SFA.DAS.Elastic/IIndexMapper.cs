using System.Threading.Tasks;
using Nest;

namespace SFA.DAS.Elastic
{
    public interface IIndexMapper
    {
        Task EnureIndexExistsAsync(string environmentName, IElasticClient client);
    }
}