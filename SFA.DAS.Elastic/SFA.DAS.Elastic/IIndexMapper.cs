using System.Threading.Tasks;
using Nest;

namespace SFA.DAS.Elastic
{
    public interface IIndexMapper
    {
        Task EnureIndexExists(IEnvironmentConfiguration config, IElasticClient client);
    }
}