using System;
using System.Collections.Generic;
using Nest;

namespace SFA.DAS.Elastic
{
    public interface IElasticClientFactory
    {
        string EnvironmentName { get; }
        IEnumerable<IIndexMapper> IndexMappers { get; }
        IConnectionSettingsValues ConnectionSettings { get; }

        IElasticClient CreateClient();
    }
}