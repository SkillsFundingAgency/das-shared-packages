using System.Threading.Tasks;
using Nest;

namespace SFA.DAS.Elastic
{
    public abstract class IndexMapper<T> : IIndexMapper where T : class
    {
        protected abstract string IndexName { get; }

        public async Task EnureIndexExists(IEnvironmentConfiguration config, IElasticClient client)
        {
            var indexName = $"{config.EnvironmentName.ToLower()}-{IndexName}";

            client.ConnectionSettings.DefaultIndices.Add(typeof(T), indexName);

            var response = await client.IndexExistsAsync(indexName).ConfigureAwait(false);

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
        }

        protected abstract void Map(TypeMappingDescriptor<T> mapper);
    }
}