using System;
using Microsoft.AspNetCore.Authentication;
using SFA.DAS.GovUK.Auth.Configuration;

namespace SFA.DAS.GovUK.Auth.AppStart
{
    public static class AuthenticationExtension
    {
        public static bool EnableVerify(GovUkOidcConfiguration config, AuthenticationProperties authenticationProperties)
        {
            var enabledByConfig = config.EnableVerify != null &&
                config.EnableVerify.Equals("true", StringComparison.CurrentCultureIgnoreCase);

            var enabledByProperties = authenticationProperties.Items.TryGetValue("enableVerify", out var flag)
                    && string.Equals(flag, true.ToString(), StringComparison.OrdinalIgnoreCase);

            return enabledByConfig || enabledByProperties;
        }

        public static bool Disable2Fa(GovUkOidcConfiguration config)
        {
            var disabled2FaByConfig = config.Disable2Fa != null &&
                config.Disable2Fa.Equals("true", StringComparison.CurrentCultureIgnoreCase);

            return disabled2FaByConfig;
        }
    }
}
