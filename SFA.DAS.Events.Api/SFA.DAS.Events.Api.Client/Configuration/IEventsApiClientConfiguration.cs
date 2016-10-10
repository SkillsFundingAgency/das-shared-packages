namespace SFA.DAS.Events.Api.Client.Configuration
{
    public interface IEventsApiClientConfiguration
    {
        string BaseUrl { get; set; }
        string ClientToken { get; set; }
    }
}