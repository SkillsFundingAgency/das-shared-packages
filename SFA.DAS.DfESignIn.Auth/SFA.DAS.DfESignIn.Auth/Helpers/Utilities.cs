using Microsoft.Extensions.Configuration;
using System;

namespace SFA.DAS.DfESignIn.Auth.Helpers
{
    public static class Utilities
    {
        public static bool IsLocalOrDev(this IConfiguration configuration)
        {
            return configuration.IsLocal() || configuration.IsDev() || configuration.IsLocalAcceptanceTests();
        }

        public static bool IsLocalAcceptanceTestsOrDev(this IConfiguration configuration)
        {
            return configuration.IsLocalAcceptanceTests() || configuration.IsDev();
        }

        public static bool IsLocalAcceptanceTests(this IConfiguration configuration)
        {
            return configuration["Environment"].Equals("LOCAL_ACCEPTANCE_TESTS", StringComparison.CurrentCultureIgnoreCase);
        }

        public static bool IsDev(this IConfiguration configuration)
        {
            return configuration["Environment"].Equals("DEV", StringComparison.CurrentCultureIgnoreCase);
        }
        public static bool IsLocal(this IConfiguration configuration)
        {
            return configuration["Environment"].Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase);
        }
    }
}