using Microsoft.IdentityModel.KeyVaultExtensions;
using Microsoft.IdentityModel.Tokens;

namespace SFA.DAS.GovUK.Auth.Services
{
    public class AzureKeyVaultSigningCredentialsProvider : ISigningCredentialsProvider
    {
        private readonly SigningCredentials _cached;

        public AzureKeyVaultSigningCredentialsProvider(string keyIdentifier, IAzureIdentityService identityService)
        {
            var key = new KeyVaultSecurityKey(keyIdentifier, identityService.AuthenticationCallback);
            _cached = new SigningCredentials(key, SecurityAlgorithms.RsaSha256)
            {
                CryptoProviderFactory = new CryptoProviderFactory { CustomCryptoProvider = new CustomKeyVaultCryptoProvider() }
            };
        }

        public SigningCredentials GetSigningCredentials() => _cached;
    }
}
