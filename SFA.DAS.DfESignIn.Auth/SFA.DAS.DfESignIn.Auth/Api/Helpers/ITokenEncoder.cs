namespace SFA.DAS.DfESignIn.Auth.Api.Helpers
{
    public interface ITokenEncoder
    {
        string Base64Encode(byte[] stringInput);
    }
}