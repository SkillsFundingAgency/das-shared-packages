namespace SFA.DAS.DfESignIn.Auth.Api
{
    public interface ITokenDataSerializer
    {
        string Serialize(object obj);
    }
}