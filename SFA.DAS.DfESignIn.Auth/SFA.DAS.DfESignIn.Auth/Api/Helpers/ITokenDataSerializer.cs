namespace SFA.DAS.DfESignIn.Auth.Api.Helpers
{
    public interface ITokenDataSerializer
    {
        string Serialize(object obj);
    }
}