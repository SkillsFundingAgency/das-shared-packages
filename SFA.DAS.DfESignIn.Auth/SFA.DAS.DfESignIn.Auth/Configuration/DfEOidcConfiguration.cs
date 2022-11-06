namespace SFA.DAS.DfESignIn.Auth.Configuration
{
    public class DfEOidcConfiguration : IDfEOidcConfiguration
    {
        public string BaseUrl { get; set; }
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public string ResponseType { get; set; }
        public string Scopes { get; set; }
        public string APIServiceUrl { get; set; }
        public string APIServiceId { get; set; }
        public string APIServiceAudience { get; set; }
        public string APIServiceSecret { get; set; }
    }

    public interface IDfEOidcConfiguration
    {
        string BaseUrl { get; set; }
        string ClientId { get; set; }
        string Secret { get; set; }
        string ResponseType { get; set; }
        string Scopes { get; set; }
        string APIServiceUrl { get; set; }
        string APIServiceId { get; set; }
        string APIServiceAudience { get; set; }
        string APIServiceSecret { get; set; }
    }
}