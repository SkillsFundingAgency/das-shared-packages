namespace SFA.DAS.Http.Configuration
{
    public interface IAzureActiveDirectoryClientConfiguration
    {
        // Azure AD Configuration
        string ApiBaseUrl { get; }
        string Tenant { get; }
        string ClientId { get; }
        string ClientSecret { get; }
        string IdentifierUri { get; }
    }
}
