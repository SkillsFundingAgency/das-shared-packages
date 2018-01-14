using System.Collections.Generic;
using Nest;

namespace SFA.DAS.Elastic
{
    public interface IElasticClientFactory
    {
        IConnectionSettingsValues ConnectionSettings { get; }
        string EnvironmentName { get; }
        IEnumerable<IIndexMapper> IndexMappers { get; }

        IElasticClient CreateClient();
    }
}