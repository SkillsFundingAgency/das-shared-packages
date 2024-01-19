// See https://aka.ms/new-console-template for more information
using SFA.DAS.Authorization.TokenGenerator.Models;
using SFA.DAS.Authorization.TokenGenerator.Providers;
using System.Reflection.Emit;
using System.Security.Claims;
using System.Text.Json;

class program
{
	static void Main(string[] args)
	{
		DisplayTitle();

		var appSettings = ConfigurationProvider.GetAppSettings();

		List<Claim>? claims = null;

		while (true)
		{
			//  Set User
			if (claims == null)
			{
				claims = SetUserClaims(appSettings);
				DisplayUser(claims);
			}

			DisplayInstructions();

			var readKey = Console.ReadKey();

			switch (readKey.Key)
			{
				case ConsoleKey.Escape:
					Environment.Exit(0);
					break;

				case ConsoleKey.Spacebar:
					claims = null;
					break;

				case ConsoleKey.Enter:
					var token = BearerTokenProvider.GetBearerToken(appSettings.UserBearerTokenSigningKey, claims);
					Console.WriteLine(token);
					break;
			}
		}
	}

	static void DisplayTitle()
	{
		Console.WriteLine("======================================================================");
		Console.WriteLine("                       Bearer Token Generator");
		Console.WriteLine("======================================================================");
	}

	static void DisplayInstructions()
	{
		Console.WriteLine("");
		Console.WriteLine("----------------------------------------------------------------------");
		Console.WriteLine("                     PRESS");
		Console.WriteLine("            Enter : To Generate New Token");
		Console.WriteLine("            Space : To Select different user type");
		Console.WriteLine("           Escape : To Exit");
		Console.WriteLine("----------------------------------------------------------------------");
		Console.WriteLine("");
	}


	static List<Claim> SetUserClaims(AppSettings appSettings)
	{
		var userType = string.Empty;

		//  Select user type
		while (userType != "P" && userType != "E")
		{
			Console.WriteLine("Enter P for provider user or E for employer user");
			userType = Console.ReadLine()?.ToUpper();
		}

		var claims = UserProvider.GenerateUserClaims(userType, appSettings);
		return claims;
	}

	static void DisplayUser(List<Claim> claims)
	{
		Console.WriteLine("");
		Console.WriteLine("User Claims");
		Console.WriteLine("");

		var maxLabelLength = claims.Max(x => x.Type.Length) + 2; // 2 spaces to indent the line

		foreach (var claim in claims)
		{
			var padding = new string(' ', maxLabelLength - claim.Type.Length);
			Console.WriteLine($"{padding}{claim.Type} : {claim.Value}");
		}
		Console.WriteLine("");
	}
}




