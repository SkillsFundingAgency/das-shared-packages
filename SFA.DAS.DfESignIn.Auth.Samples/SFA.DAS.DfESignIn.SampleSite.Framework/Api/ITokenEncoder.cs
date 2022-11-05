namespace SFA.DAS.DfESignIn.SampleSite.Framework.Api
{
    public interface ITokenEncoder
    {
        string Base64Encode(byte[] stringInput);
    }
}