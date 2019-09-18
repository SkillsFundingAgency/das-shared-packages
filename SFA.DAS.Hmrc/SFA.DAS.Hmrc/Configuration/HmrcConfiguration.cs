namespace SFA.DAS.Hmrc.Configuration
{
    public class HmrcConfiguration : IHmrcConfiguration
    {
        public string BaseUrl { get; set; }
        public string ClientId { get; set; }
        public string Scope { get; set; }
        public string ClientSecret { get; set; }
        public string ServerToken { get; set; }
        public string OgdSecret { get; set; }
        public string OgdClientId { get; set; }
        public string AzureClientId { get; set; }
        public string AzureAppKey { get; set; }
        public string AzureResourceId { get; set; }
        public string AzureTenant { get; set; }
        public bool UseHiDataFeed { get; set; }
    }
}