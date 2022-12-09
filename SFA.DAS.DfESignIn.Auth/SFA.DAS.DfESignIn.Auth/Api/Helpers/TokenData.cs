using System;
using System.Collections.Generic;

namespace SFA.DAS.DfESignIn.Auth.Api.Helpers
{
    public class TokenData
    {
        public Dictionary<string, object> Header { get; set; }
        public Dictionary<string, object> Payload { get; set; }
        public TokenData()
        {
            Header = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
            Payload = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        }
    }

}
