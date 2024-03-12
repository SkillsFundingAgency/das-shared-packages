namespace SFA.DAS.Authorization.TokenGenerator.Models;

public class AppSettings
{
    public string? UserBearerTokenSigningKey { get; set; }
    public ProviderUser? ProviderUser { get; set; }
    public EmployerUser? EmployerUser { get; set; }
}
