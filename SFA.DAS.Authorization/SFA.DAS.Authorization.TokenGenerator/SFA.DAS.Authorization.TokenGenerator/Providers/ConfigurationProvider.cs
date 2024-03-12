using SFA.DAS.Authorization.TokenGenerator.Models;
using System.Text.Json;

namespace SFA.DAS.Authorization.TokenGenerator.Providers;

internal static class ConfigurationProvider
{
	internal static AppSettings GetAppSettings()
	{
		string jsonString = File.ReadAllText("appsettings.json");
		var options = new JsonSerializerOptions
		{
			PropertyNamingPolicy = null,
			PropertyNameCaseInsensitive = true,
		};
		var appSettings = JsonSerializer.Deserialize<AppSettings>(jsonString, options);

		return appSettings ?? new AppSettings();
	}
}
