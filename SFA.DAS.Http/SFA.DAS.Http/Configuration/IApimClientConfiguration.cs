namespace SFA.DAS.Http.Configuration
{
    public interface IApimClientConfiguration
    {
        string ApiBaseUrl { get; }
        string SubscriptionKey { get; }
        string ApiVersion { get; }
        
    }
}
