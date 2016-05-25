using System.Security.Cryptography;
using System.Text;

namespace SFA.DAS.CodeGenerator
{
    public class RandomCodeGenerator : ICodeGenerator
    {
        private readonly RandomNumberGenerator _randomNumberGenerator;

        public RandomCodeGenerator(RandomNumberGenerator randomNumberGenerator = null)
        {
            if (randomNumberGenerator == null)
            {
                _randomNumberGenerator = RandomNumberGenerator.Create();
            }
            else
            {
                _randomNumberGenerator = randomNumberGenerator;
            }
            
        }

        public const string Numerics = "123456789"; 

        public const string Alphanumerics = "46789BCDFGHJKLMNPRSTVWXY";


        public string GenerateAlphaNumeric(int length = 6)
        {
            return Generate(Alphanumerics, length);
        }

        public string GenerateNumeric(int length = 4)
        {
            return Generate(Numerics, length);
        }

        private string Generate(string characters, int length)
        {
            var bytes = GenerateRandomBytes(length);
            var sb = new StringBuilder(length);

            for (var i = 0; i < length; i++)
            {
                var index = bytes[i] % characters.Length;

                sb.Append(characters[index]);
            }

            return sb.ToString();
        }

        private byte[] GenerateRandomBytes(int length)
        {
            var rng = _randomNumberGenerator;
            var bytes = new byte[length];

            rng.GetBytes(bytes);

            return bytes;
        }
        
    }


}
