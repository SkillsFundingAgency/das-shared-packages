namespace SFA.DAS.Http.Configuration
{
    public interface IManagedIdentityClientConfiguration
    {
        string ApiBaseUrl { get; }
        string IdentifierUri { get; }
    }
}
