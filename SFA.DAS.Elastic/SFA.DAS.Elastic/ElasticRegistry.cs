using Microsoft.Azure;
using Nest;
using StructureMap;

namespace SFA.DAS.Elastic
{
    public class ElasticRegistry : Registry
    {
        private static readonly object Lock = new object();

        private static IElasticClientFactory _elasticClientFactory;

        public ElasticRegistry()
        {
            var environmentName = CloudConfigurationManager.GetSetting("EnvironmentName");
            var environmentConfig = new EnvironmentConfiguration { EnvironmentName = environmentName };

            For<IElasticClient>().Use(c => c.GetInstance<IElasticClientFactory>().GetClient());
            For<IElasticClientFactory>().Use(c => GetElasticClientFactory(c)).Singleton();
            For<IEnvironmentConfiguration>().Use(environmentConfig);

            Scan(s =>
            {
                s.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS"));
                s.AddAllTypesOf<IIndexMapper>();
            });
        }

        private IElasticClientFactory GetElasticClientFactory(IContext context)
        {
            lock (Lock)
            {
                if (_elasticClientFactory == null)
                {
                    _elasticClientFactory = context.GetInstance<ElasticClientFactory>();
                }
            }

            return _elasticClientFactory;
        }
    }
}