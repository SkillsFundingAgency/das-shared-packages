namespace SFA.DAS.DfESignIn.Auth.Configuration
{
    public class DfEOidcConfiguration : IDfEOidcConfiguration
    {
        public string BaseUrl { get; set; }
        public string ClientId { get; set; }
        public string KeyVaultIdentifier { get; set; }
        public string Secret { get; set; }
        public string ResponseType { get; set; }
        public string Scopes { get; set; }
        public string APIServiceUrl { get; set; }
        public string APIServiceAudience { get; set; }
        public string APIServiceSecret { get; set; }
    }
}