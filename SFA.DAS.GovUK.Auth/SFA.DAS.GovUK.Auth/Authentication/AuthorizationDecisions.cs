using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.GovUK.Auth.Authentication
{
    [ExcludeFromCodeCoverage]
    public static class AuthorizationDecisions
    {
        public static string Suspended => "Suspended";
        public static string Allowed => "Allowed";
    }
}