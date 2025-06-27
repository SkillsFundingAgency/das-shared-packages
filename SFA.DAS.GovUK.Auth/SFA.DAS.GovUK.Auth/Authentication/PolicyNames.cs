using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.GovUK.Auth.Authentication
{
    [ExcludeFromCodeCoverage]
    public static class PolicyNames
    {
        public static string IsAuthenticated => nameof(IsAuthenticated);
        public static string IsActiveAccount => nameof(IsActiveAccount);
        public static string IsVerified => nameof(IsVerified);
    }
}