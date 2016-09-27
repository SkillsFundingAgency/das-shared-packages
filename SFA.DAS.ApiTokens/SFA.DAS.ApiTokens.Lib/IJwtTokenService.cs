using System;
using System.Collections.Generic;

namespace SFA.DAS.ApiTokens.Lib
{
    public interface IJwtTokenService
    {
        string Encode(string data, string audience, string issuer, double lifetimeDurationSeconds = 60);
        string Decode(string token, IEnumerable<string> validAudiences, IEnumerable<string> validIssuers);
    }
}
