using Microsoft.Extensions.Configuration;

namespace SFA.DAS.NServiceBus.Extensions;

public static class ConfigurationExtensions
{
    public static string NServiceBusConnectionString(this IConfiguration config) => config["NServiceBusConnectionString"] ?? "UseLearningEndpoint=true";
    public static string NServiceBusLicense(this IConfiguration config) => config["NServiceBusLicense"];
    public static string NServiceBusFullNamespace(this IConfiguration config) => config["AzureWebJobsServiceBus:fullyQualifiedNamespace"];
    public static string NServiceBusSASConnectionString(this IConfiguration config) => config["AzureWebJobsServiceBus"];
}