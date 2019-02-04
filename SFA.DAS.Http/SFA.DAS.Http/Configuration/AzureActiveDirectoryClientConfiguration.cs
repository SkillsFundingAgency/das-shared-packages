namespace SFA.DAS.Http.Configuration
{
    public class AzureActiveDirectoryClientConfiguration : IAzureActiveDirectoryClientConfiguration
    {
        public string ApiBaseUrl { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string IdentifierUri { get; set; }
        public string Tenant { get; set; }
    }
}