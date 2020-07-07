using System.Collections.Generic;

namespace SFA.DAS.Encoding.Mvc
{
    public interface IAutoDecodeMappingProvider
    {
        Dictionary<string, AutoDecodeMapping> AutoDecodeMappings { get; }
    }
}
