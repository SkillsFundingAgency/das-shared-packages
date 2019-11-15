namespace SFA.DAS.Authentication
{
    public class IdentityServerConfiguration 
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string BaseAddress { get; set; }
        public string AuthorizeEndPoint { get; set; }
        public string LogoutEndpoint { get; set; }
        public string TokenEndpoint { get; set; }
        public string UserInfoEndpoint { get; set; }
        
        public bool UseCertificate { get; set; }
        public string Scopes { get; set; }
        public ClaimIdentifierConfiguration ClaimIdentifierConfiguration { get; set; }
        public string ChangePasswordLink { get; set; }
        public string ChangeEmailLink { get; set; }
        public string RegisterLink { get; set; }
        public string AccountActivationUrl { get; set; }
    }
}