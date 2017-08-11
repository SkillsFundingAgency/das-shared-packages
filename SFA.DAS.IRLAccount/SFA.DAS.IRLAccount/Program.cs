using System;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace SFA.DAS.IRLAccount
{
    class Program
    {
        private static string getSalt()
        {
            var random = new RNGCryptoServiceProvider();
            int max_length = 32;
            byte[] salt = new byte[max_length];
            random.GetNonZeroBytes(salt);
            return Convert.ToBase64String(salt);
        }
        static void Main(string[] args)
        {

            Console.Write("PasswordKey:");
            var pwdkey = Console.ReadLine();

            Console.Write("Password:");
            var plaintext = Console.ReadLine();

            var salt = getSalt();

            var saltedPassword = Convert.FromBase64String(salt).Concat(Encoding.Unicode.GetBytes(plaintext)).ToArray();
            HMAC hasher = new HMACSHA256(Convert.FromBase64String(pwdkey));
            var hash = hasher.ComputeHash(saltedPassword);
            var pbkdf2 = new Rfc2898DeriveBytes(Convert.ToBase64String(hash), Convert.FromBase64String(salt), 1000);
            hash = pbkdf2.GetBytes(hash.Length);

            Console.WriteLine("Hashed Password:");

            Console.WriteLine(Convert.ToBase64String(hash));

            Console.WriteLine("Salt:");

            Console.WriteLine(salt);

            Console.ReadLine();
        }
    }
}
