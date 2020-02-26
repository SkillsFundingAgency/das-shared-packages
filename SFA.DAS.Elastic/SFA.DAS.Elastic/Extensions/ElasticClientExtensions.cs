using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SFA.DAS.Elastic.Extensions
{
    public static class ElasticClientExtensions
    {
        public static List<IIndexMapper> GetIndexMappers(this Assembly assembly)
        {
            var indexMappers = new List<IIndexMapper>();
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            if (assembly != null)
            {
                indexMappers.AddRange(assembly
                    .GetExportedTypes()
                    .Where(t =>
                        !t.IsAbstract &&
                        !t.IsInterface &&
                        typeof(IIndexMapper).IsAssignableFrom(t)
                    )
                    .Select(Activator.CreateInstance)
                    .Cast<IIndexMapper>());
            }
            return indexMappers;
        }

        public static async Task CreateIndicesIfNotExistsAsync(this IElasticClient client, IEnumerable<IIndexMapper> indexMappers, string environmentName = "")
        {
            foreach (var m in indexMappers)
            {
                await m.EnureIndexExistsAsync(client, environmentName);
                m.Dispose();
            }
        }
    }
}