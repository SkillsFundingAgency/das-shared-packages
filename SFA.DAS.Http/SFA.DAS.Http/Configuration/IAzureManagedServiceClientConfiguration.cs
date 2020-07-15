namespace SFA.DAS.Http.Configuration
{
    public interface IAzureManagedServiceClientConfiguration
    {
        string ApiBaseUrl { get; }
        string SubscriptionKey { get; }
    }
}
