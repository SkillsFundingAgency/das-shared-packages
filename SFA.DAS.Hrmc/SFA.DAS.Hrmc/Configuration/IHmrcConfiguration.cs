namespace SFA.DAS.Hrmc.Configuration
{
    public interface IHmrcConfiguration
    {
        string BaseUrl { get; set; }
        string ClientId { get; set; }
        string Scope { get; set; }
        string ClientSecret { get; set; }
        string ServerToken { get; set; }
        string OgdSecret { get; set; }
        string OgdClientId { get; set; }
        string AzureClientId { get; set; }
        string AzureAppKey { get; set; }
        string AzureResourceId { get; set; }
        string AzureTenant { get; set; }
        bool UseHiDataFeed { get; set; }
    }
}