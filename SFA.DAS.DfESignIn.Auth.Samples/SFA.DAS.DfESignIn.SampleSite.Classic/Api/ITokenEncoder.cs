namespace SFA.DAS.DfESignIn.SampleSite.Classic.Api
{
    public interface ITokenEncoder
    {
        string Base64Encode(byte[] stringInput);
    }
}