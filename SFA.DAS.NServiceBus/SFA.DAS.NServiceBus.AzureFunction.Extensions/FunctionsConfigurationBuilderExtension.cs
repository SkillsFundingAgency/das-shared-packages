using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace SFA.DAS.NServiceBus.AzureFunction.Extensions;

public static class FunctionsConfigurationBuilderExtension
{
    public static void ConfigureServiceBusManagedIdentity(
        this IFunctionsConfigurationBuilder builder,
        string connectionStringName = "AzureWebJobsServiceBus")
    {
        var preConfig = builder.ConfigurationBuilder.Build();

        var key = $"{connectionStringName}__fullyQualifiedNamespace";
        var serviceBusNamespace = preConfig.GetValue<string>(key);
        if (serviceBusNamespace != null)
        {
            builder.ConfigurationBuilder.AddInMemoryCollection(
                new Dictionary<string, string>
                {
                    {
                        connectionStringName,
                        $"Endpoint=sb://{serviceBusNamespace}/;Authentication=Managed Identity;"
                    }
                });
        }
    }
}