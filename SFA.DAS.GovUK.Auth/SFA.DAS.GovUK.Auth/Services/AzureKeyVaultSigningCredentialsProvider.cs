using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.KeyVaultExtensions;
using Microsoft.IdentityModel.Tokens;
using SFA.DAS.GovUK.Auth.Configuration;

namespace SFA.DAS.GovUK.Auth.Services
{
    public class AzureKeyVaultSigningCredentialsProvider : ISigningCredentialsProvider
    {
        private readonly SigningCredentials _cached;

        public AzureKeyVaultSigningCredentialsProvider(IOptions<GovUkOidcConfiguration> options, IAzureIdentityService identityService)
        {
            var config = options.Value;
            var key = new KeyVaultSecurityKey(config.KeyVaultIdentifier, identityService.AuthenticationCallback);
            _cached = new SigningCredentials(key, SecurityAlgorithms.RsaSha512)
            {
                CryptoProviderFactory = new CryptoProviderFactory { CustomCryptoProvider = new CustomKeyVaultCryptoProvider() }
            };
        }

        public SigningCredentials GetSigningCredentials() => _cached;
    }
}
