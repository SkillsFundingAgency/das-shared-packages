namespace SFA.DAS.Commitments.Api.Client.Configuration
{
    public interface ICommitmentsApiClientConfiguration
    {
        string BaseUrl { get; set; }
        string ClientSecret { get; set; }
    }
}