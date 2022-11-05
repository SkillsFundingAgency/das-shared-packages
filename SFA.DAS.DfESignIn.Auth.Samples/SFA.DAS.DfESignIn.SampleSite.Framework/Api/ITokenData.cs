using System.Collections.Generic;

namespace SFA.DAS.DfESignIn.SampleSite.Framework.Api
{
    public interface ITokenData
    {
        IDictionary<string, object> Header { get; set; }
        IDictionary<string, object> Payload { get; set; }
    }
}