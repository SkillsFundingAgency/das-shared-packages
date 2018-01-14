using System.Threading;
using System.Threading.Tasks;
using Nest;

namespace SFA.DAS.Elastic
{
    public abstract class IndexMapper<T> : IIndexMapper where T : class
    {
        protected abstract string IndexName { get; }
        
        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);

        public async Task EnureIndexExistsAsync(string environmentName, IElasticClient client)
        {
            var type = typeof(T);
            var indexName = $"{environmentName.ToLower()}-{IndexName}";

            await _mutex.WaitAsync().ConfigureAwait(false);

            try
            {
                if (!client.ConnectionSettings.DefaultIndices.TryGetValue(type, out var defaultIndexName) || indexName != defaultIndexName)
                {
                    var response = await client.IndexExistsAsync(indexName);

                    if (!response.Exists)
                    {
                        await client.CreateIndexAsync(indexName, i => i
                            .Mappings(ms => ms
                                .Map<T>(m =>
                                {
                                    Map(m);
                                    return m;
                                })
                            )
                        );
                    }

                    client.ConnectionSettings.DefaultIndices[type] = indexName;
                }
            }
            finally
            {
                _mutex.Release();
            }
        }

        public void Dispose()
        {
            _mutex.Dispose();
        }

        protected abstract void Map(TypeMappingDescriptor<T> mapper);
    }
}