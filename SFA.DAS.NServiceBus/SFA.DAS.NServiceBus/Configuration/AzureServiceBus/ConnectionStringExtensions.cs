namespace SFA.DAS.NServiceBus.Configuration.AzureServiceBus;

public static class ConnectionStringExtensions
{
    public static string FormatConnectionString(this string connectionString)
    {
        return connectionString.Replace("Endpoint=sb://", string.Empty).TrimEnd('/');
    }
}