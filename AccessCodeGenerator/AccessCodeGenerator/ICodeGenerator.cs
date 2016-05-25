namespace SFA.DAS.CodeGenerator
{
    public interface ICodeGenerator
    {
        string GenerateAlphaNumeric(int length = 6);

        string GenerateNumeric(int length = 4);
    }
}