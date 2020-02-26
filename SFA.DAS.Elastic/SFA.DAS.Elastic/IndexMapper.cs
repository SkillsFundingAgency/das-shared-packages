using System;
using System.Threading;
using System.Threading.Tasks;
using Nest;

namespace SFA.DAS.Elastic
{
    public abstract class IndexMapper<T> : IIndexMapper where T : class
    {
        private readonly string _indexName;

        private readonly SemaphoreSlim _mutex = new SemaphoreSlim(1, 1);

        public IndexMapper(string indexName)
        {
            _indexName = indexName;
        }

        public async Task EnureIndexExistsAsync(IElasticClient client, string environmentName = "")
        {
            var type = typeof(T);
            var indexName = string.IsNullOrEmpty(environmentName) ? _indexName : $"{environmentName.ToLower()}-{_indexName}";

            await _mutex.WaitAsync();

            try
            {
                if (!client.ConnectionSettings.DefaultIndices.TryGetValue(type, out var defaultIndexName) || indexName != defaultIndexName)
                {
                    var response = await client.Indices.ExistsAsync(indexName).ConfigureAwait(false);

                    if (!response.Exists)
                    {
                        await client.Indices.CreateAsync(indexName, i => i
                            .Map<T>(m =>
                            {
                                Map(m);
                                return m;
                            })
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
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _isDisposed;
        protected virtual void Dispose(bool disposing) 
        {
            if (_isDisposed) return;

            if (disposing)
            {
                _mutex.Dispose();
            }

            _isDisposed = true;
        }

        protected abstract void Map(TypeMappingDescriptor<T> mapper);
    }
}