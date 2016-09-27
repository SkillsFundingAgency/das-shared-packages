using System;
using System.Text;
using SFA.DAS.ApiTokens.Lib;

namespace SFA.DAS.ApiTokens.Console
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                var arguments = Arguments.Parse(args);

                // check for help request
                if (arguments.ContainsKey("?") || arguments.ContainsKey("H") || arguments.ContainsKey("HELP"))
                {
                    ShowHelp();

                    Environment.Exit(1);
                }

                // parse args
                var issuer = arguments.GetValueOrDefault(string.Empty, "issuer");
                var audience = arguments.GetValueOrDefault(string.Empty, "audience");
                var data = arguments.GetValueOrDefault(string.Empty, "data");
                var secret = arguments.GetValueOrDefault(string.Empty, "secret");
                var duration = int.Parse(arguments.GetValueOrDefault("720", "duration"));

                // create token
                var encoder = new JwtTokenService(secret);
                var token = encoder.Encode(data, audience, issuer, TimeSpan.FromHours(duration).TotalSeconds);

                System.Console.WriteLine(token);

                Environment.Exit(0);
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("ERROR " + ex.FormatException());

                Environment.Exit(1);
            }
        }

        private static void ShowHelp()
        {
            var sb = new StringBuilder();

            sb.AppendLine("\nAPITokenGen command line help");
            sb.AppendLine("\nThis tool generates a JWT for granting access to a single client.");
            sb.AppendLine("The generated token is encoded and signed (using the HS256 algorithm). It is NOT encrypted.");
            sb.AppendLine("\nThe following command line arguments are supported (all case insensitive, any order):");
            sb.AppendLine("\n  /issuer:<value>    - <value> is the URI of the issuer (the API service)");
            sb.AppendLine("  /audience:<value>  - <value> is the URI of the audience (the API client/consumer)");
            sb.AppendLine("  /data:<value>      - <value> is a space delimited list of roles/claims/permissions that the token will contain");
            sb.AppendLine("  /secret:<value>    - <value> is a phrase used to sign the token (minimum 16 characters)");
            sb.AppendLine("  /duration:<value>  - <value> is the number of hours until the token expires, defaults to 720 (30 days)");
            sb.AppendLine("\nExample:");
            sb.AppendLine("\n  APITokenGen /issuer:http://server.net /audience:http://client.net /data:\"Role1 Role2\" /secret:\"Some Super Secret\" /duration:180");

            System.Console.WriteLine(sb.ToString());
        }
    }
}
