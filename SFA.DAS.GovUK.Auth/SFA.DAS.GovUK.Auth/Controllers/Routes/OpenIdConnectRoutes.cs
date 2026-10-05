namespace SFA.DAS.GovUK.Auth.Controllers.Routes
{
    public static class OpenIdConnectRoutes
    {
        /// <summary>
        /// Paths which are intercepted by OpenId connect middleware these raise
        /// events which are handled by <see cref="GovUkOpenIdConnectEvents"/>
        /// </summary>
        public static class Paths
        {
            public const string SignedOutCallback = "/signed-out";
            public const string SignInCallback = "/sign-in";
        }
    }
}