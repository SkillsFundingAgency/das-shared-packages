using SFA.DAS.NLog.Logger;
using System.Collections.Generic;

namespace SFA.DAS.Authentication.Extensions
{
    public class MixedModeAuthenticationOptions
    {
        public IEnumerable<string> ValidIssuers { get; set; }
        public IEnumerable<string> ValidAudiences { get; set; }
        public string ApiTokenSecret { get; set; }
        public string MetadataEndpoint { get; set; }
        public ILog Logger { get; set; }
    }
}
