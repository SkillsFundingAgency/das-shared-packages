namespace SFA.DAS.GovUK.SampleSite.Controllers.Routes
{
    public static class StubRoutes
    {
        public static class Names
        {
            public const string SignIn = nameof(SignIn);
            public const string SignedIn = nameof(SignedIn);
        }

        public static class Paths
        {
            public const string Controller = "stub";
            public const string SignIn = "sign-in-stub";
            public const string SignedIn = "signed-in-stub";
        }

        public static string StubControllerPath(this string path)
        {
            return $"/{Paths.Controller}/{path}";
        }
    }
}