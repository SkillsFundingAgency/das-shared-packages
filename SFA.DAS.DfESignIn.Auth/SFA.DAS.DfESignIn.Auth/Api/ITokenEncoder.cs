namespace SFA.DAS.DfESignIn.Auth.Api
{
    public interface ITokenEncoder
    {
        string Base64Encode(byte[] stringInput);
    }
}