using System.Collections.Generic;

namespace SFA.DAS.DfESignIn.Auth.Api.Helpers
{
    public interface ITokenData
    {
        IDictionary<string, object> Header { get; set; }
        IDictionary<string, object> Payload { get; set; }
    }
}