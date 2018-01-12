using System;
using System.Collections.Generic;
using Nest;

namespace SFA.DAS.Elastic
{
    public interface IElasticClientFactory
    {
        string EnvironmentName { get; }
        Func<IEnumerable<IIndexMapper>> IndexMappersFactory { get; }
        IConnectionSettingsValues ConnectionSettings { get; }

        IElasticClient CreateClient();
    }
}