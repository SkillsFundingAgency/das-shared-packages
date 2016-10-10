namespace SFA.DAS.Notifications.Api.Client.Configuration
{
    public interface INotificationsApiClientConfiguration
    {
        string BaseUrl { get; set; }
        string ClientToken { get; set; }
    }
}