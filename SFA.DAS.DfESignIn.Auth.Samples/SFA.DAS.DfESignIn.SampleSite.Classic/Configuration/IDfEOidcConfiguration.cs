namespace SFA.DAS.DfESignIn.SampleSite.Classic
{
    public interface IDfEOidcConfiguration
    {
        string BaseUrl { get; set; }
        string ClientId { get; set; }
        string KeyVaultIdentifier { get; set; }
        string Secret { get; set; }
        string ResponseType { get; set; }
        string Scopes { get; set; }
        string APIServiceUrl { get; set; }
        string APIServiceId { get; set; }
        string APIServiceAudience { get; set; }
        string APIServiceSecret { get; set; }
    }
}