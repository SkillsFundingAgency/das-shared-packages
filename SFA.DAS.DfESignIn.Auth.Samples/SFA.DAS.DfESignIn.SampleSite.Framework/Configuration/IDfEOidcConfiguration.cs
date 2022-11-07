namespace SFA.DAS.DfESignIn.SampleSite.Framework
{
    public interface IDfEOidcConfiguration
    {
        string BaseUrl { get; set; }
        string ClientId { get; set; }
        string Secret { get; set; }
        string APIServiceUrl { get; set; }
        string APIServiceId { get; set; }
        string APIServiceSecret { get; set; }
    }
}