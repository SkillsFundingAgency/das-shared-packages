namespace SFA.DAS.Events.Api.Client.Configuration
{
    public interface IEventsApiClientConfiguration
    {
        string BaseUrl { get; }
        string ClientToken { get; }
    }
}