using Microsoft.Extensions.Configuration;

namespace SFA.DAS.NServiceBus.Extensions;

public static class ConfigurationExtensions
{
    public static string NServiceBusConnectionString(this IConfiguration config) => config["NServiceBusConnectionString"] ?? "UseLearningEndpoint=true";

    public static string NServiceBusLicense(this IConfiguration config) => config["NServiceBusLicense"];
}