namespace SFA.DAS.GovUK.Auth.Configuration
{
    public class GovUkOidcConfiguration
    {
        public string BaseUrl { get; set; }
        public string ClientId { get; set; }
        public string KeyVaultIdentifier { get; set; }
    }

    public static class GovUkConstants
    {
        public const string StubAuthCookieName = "SFA.Apprenticeships.StubAuthCookie";
    }
}