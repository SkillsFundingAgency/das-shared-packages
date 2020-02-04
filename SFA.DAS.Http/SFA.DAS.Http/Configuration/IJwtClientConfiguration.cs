namespace SFA.DAS.Http.Configuration
{
    public interface IJwtClientConfiguration
    {
        // JWT Configuration
        string Issuer {get; }
        string Audience {get; }
        string ClientSecret {get; }
        int TokenExpirySeconds {get; }
    }
}
