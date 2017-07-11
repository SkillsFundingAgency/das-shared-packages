namespace SFA.DAS.Http
{
    public interface IApiClientConfiguration
    {
        // Azure AD Configuration
        string Tenant { get; set; }
        string ClientId { get; set; }
        string ClientSecret { get; set; }
        string IdentifierUri { get; set; }

        // JWT Configuration
        string ClientToken { get; set; }
    }
}
