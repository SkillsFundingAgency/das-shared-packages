namespace SFA.DAS.CodeGenerator
{
    public class StaticCodeGenerator : ICodeGenerator
    {
        private const string DefaultAlphanumericCode = "ABC123";
        private const string DefaultNumericCode = "1234";

        public string GenerateAlphaNumeric(int length = 6)
        {
            return DefaultAlphanumericCode;
        }

        public string GenerateNumeric(int length = 4)
        {
            return DefaultNumericCode;
        }
    }
}