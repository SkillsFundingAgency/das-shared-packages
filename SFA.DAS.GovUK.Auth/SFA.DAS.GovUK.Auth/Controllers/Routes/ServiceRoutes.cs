namespace SFA.DAS.GovUK.Auth.Controllers.Routes
{
    public static class ServiceRoutes
    {
        public static class Names
        {
            public const string VerifyIdentity = UniqueName + nameof(VerifyIdentity);
            public const string AccountUnavailable = UniqueName + nameof(AccountUnavailable);
            public const string AccountDetails = UniqueName + nameof(AccountDetails);
            public const string UserSignedOut = UniqueName + nameof(UserSignedOut);
            public const string KeepAlive = UniqueName + nameof(KeepAlive);

            private const string UniqueName = "GovUK.Auth.";
        }

        public static class Paths
        {
            public const string Controller = "service";
            public const string VerifyIdentity = "verify-identity"; // this is more like an identity route
            public const string AccountUnavailable = "account-unavailable";
            public const string AccountDetails = "account-details";
            public const string UserSignedOut = "user-signed-out";
            public const string KeepAlive = "keepalive";
        }

        public static string ServiceControllerPath(this string path)
        {
            return $"/{Paths.Controller}/{path}";
        }
    }
}
