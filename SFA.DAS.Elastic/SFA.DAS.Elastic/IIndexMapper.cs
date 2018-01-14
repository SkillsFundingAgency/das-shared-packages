using System;
using System.Threading.Tasks;
using Nest;

namespace SFA.DAS.Elastic
{
    public interface IIndexMapper : IDisposable
    {
        Task EnureIndexExistsAsync(string environmentName, IElasticClient client);
    }
}