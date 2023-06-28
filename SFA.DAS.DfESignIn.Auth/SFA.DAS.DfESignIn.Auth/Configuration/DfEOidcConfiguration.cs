namespace SFA.DAS.DfESignIn.Auth.Configuration
{
    public class DfEOidcConfiguration
    {
        public string BaseUrl { get; set; }
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public string APIServiceUrl { get; set; }
        public string APIServiceId { get; set; }
        public string APIServiceSecret { get; set; }
        public int LoginSlidingExpiryTimeOutInMinutes { get; set; } = 30;
        public string DfELoginSessionConnectionString { get; set; }
    }
}