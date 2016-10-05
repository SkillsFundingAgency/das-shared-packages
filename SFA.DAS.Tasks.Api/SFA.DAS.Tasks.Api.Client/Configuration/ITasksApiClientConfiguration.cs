namespace SFA.DAS.Tasks.Api.Client.Configuration
{
    public interface ITasksApiClientConfiguration
    {
        string BaseUrl { get; set; }
        string ClientToken { get; set; }
    }
}
