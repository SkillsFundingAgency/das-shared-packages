namespace SFA.DAS.GovUK.SampleSite.Controllers.Routes
{
    public static class HomeRoutes
    {
        public static class Names
        {
            public const string Home = nameof(Home);
            public const string Start = nameof(Start);
            public const string AccountDetails = nameof(AccountDetails);
            public const string ActiveStatus = nameof(ActiveStatus);
            public const string VerifiedAccountDetails = nameof(VerifiedAccountDetails);
            public const string ExplainVerify = nameof(ExplainVerify);
            public const string SignOut = nameof(SignOut);
            public const string SignedOut = nameof(SignedOut);
            public const string Suspended = nameof(Suspended);
        }

        public static class Paths
        {
            public const string Controller = "";

            public const string Home = "home";
            public const string Start = "start";
            public const string AccountDetails = "account-details";
            public const string ActiveStatus = "active";
            public const string VerifiedAccountDetails = "verified-account-details";
            public const string ExplainVerify = "explain-verify";
            public const string SignOut = "sign-out";
            public const string SignedOut = "user-signed-out";
            public const string Suspended = "user-suspended";
        }

        public static string HomeControllerPath(this string path)
        {
            return $"/{path}";
        }
    }
}