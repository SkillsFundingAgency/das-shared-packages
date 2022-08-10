namespace SFA.DAS.GovUK.Auth.Configuration;

public class GovUkOidcConfiguration
{
    public string BaseUrl { get; set; } = null!;
    public string ClientId { get; set; } = null!;
    public string KeyVaultIdentifier { get; set; } = null!;
}