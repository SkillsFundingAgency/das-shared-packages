namespace SFA.DAS.GovUK.Auth.Controllers.Routes
{
    public static class ServiceRoutes
    {
        public static class Names
        {
            public const string VerifyIdentity = nameof(VerifyIdentity);
            public const string AccountUnavailable = nameof(AccountUnavailable);
            public const string AccountDetails = nameof(AccountDetails);
            public const string UserSignedOut = nameof(UserSignedOut);
            public const string KeepAlive = nameof(KeepAlive);
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
