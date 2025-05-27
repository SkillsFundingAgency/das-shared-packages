using Microsoft.IdentityModel.Tokens;

namespace SFA.DAS.GovUK.Auth.Services
{
    public interface ISigningCredentialsProvider
    {
        SigningCredentials GetSigningCredentials();
    }
}
