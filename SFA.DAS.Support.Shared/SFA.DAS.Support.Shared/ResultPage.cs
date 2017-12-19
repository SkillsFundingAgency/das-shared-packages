using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;

namespace SFA.DAS.Support.Shared
{
    [ExcludeFromCodeCoverage]
    public class ResultPage<T>
    {
        [JsonProperty("_links")]
        public PageLinks Links { get; set; }

        public IEnumerable<T> Results { get; set; }
        public int Size { get; set; }
        public int Start { get; set; }
    }
}