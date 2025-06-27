using System;
using Microsoft.AspNetCore.Authentication;
using SFA.DAS.GovUK.Auth.Configuration;

namespace SFA.DAS.GovUK.Auth.AppStart
{
    public static class AuthenticationExtension
    {
        /// <summary>
        /// Determines if the verify was either enabled on first authentication or later by a property
        /// in a subsequent challenge
        /// </summary>
        /// <param name="config"></param>
        /// <param name="authenticationProperties"></param>
        /// <returns>True; if verify is currently enabled; otherwise false</returns>
        public static bool EnableVerify(GovUkOidcConfiguration config, AuthenticationProperties authenticationProperties)
        {
            var enabledByConfig = config.EnableVerify != null &&
                config.EnableVerify.Equals("true", StringComparison.CurrentCultureIgnoreCase);

            var enabledByProperties = authenticationProperties.Items.TryGetValue("enableVerify", out var flag)
                    && string.Equals(flag, true.ToString(), StringComparison.OrdinalIgnoreCase);

            return enabledByConfig || enabledByProperties;
        }

        /// <summary>
        /// Determines if 2FA is enabled, note that when 2FA is disabled, attempting to enable verify
        /// will fail as this would be an invalid credential trust level for the verify level of confidence
        /// </summary>
        /// <param name="config"></param>
        /// <returns>True; when 2FA is disabled; otherwise false</returns>
        public static bool Disable2Fa(GovUkOidcConfiguration config)
        {
            var disabled2FaByConfig = config.Disable2Fa != null &&
                config.Disable2Fa.Equals("true", StringComparison.CurrentCultureIgnoreCase);

            return disabled2FaByConfig;
        }
    }
}
