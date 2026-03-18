using Azure.Monitor.OpenTelemetry.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace SFA.DAS.SharedOuterApi.AppStart;

public static class AddOpenTelemetryExtensions
{
    /// <summary>
    /// Add the OpenTelemetry telemetry service to the application.
    /// </summary>
    /// <param name="services">Service Collection</param>
    /// <param name="appInsightsConnectionString">Azure app insights connection string.</param>
    public static void AddOpenTelemetryRegistration(this IServiceCollection services, string appInsightsConnectionString)
    {
        if (!string.IsNullOrEmpty(appInsightsConnectionString))
        {
            // This service will collect and send telemetry data to Azure Monitor.
            services.AddOpenTelemetry().UseAzureMonitor(options =>
            {
                options.ConnectionString = appInsightsConnectionString;
            });
        }
    }

    /// <summary>
    /// Add the OpenTelemetry telemetry service to the application.
    /// </summary>
    /// <param name="services">Service Collection</param>
    /// <param name="appInsightsConnectionString">Azure app insights connection string.</param>
    /// <param name="serviceNamespace">Namespace of the service</param>
    /// <param name="serviceMeterName">Meter name</param>
    /// <param name="serviceName">Name of the service</param>
    public static void AddOpenTelemetryRegistration(
        this IServiceCollection services,
        string appInsightsConnectionString, string serviceNamespace, string serviceMeterName, string serviceName)
    {
        if (!string.IsNullOrEmpty(appInsightsConnectionString))
        {
            // This service will collect and send telemetry data to Azure Monitor.
            services.AddOpenTelemetry().UseAzureMonitor(options =>
                {
                    options.ConnectionString = appInsightsConnectionString;
                })
                // Configure metrics
                .WithMetrics(opts => opts
                    .SetResourceBuilder(ResourceBuilder.CreateDefault().AddService(
                        serviceName,
                        serviceNamespace))
                    .AddMeter(serviceMeterName));
        }
    }

    public static IServiceCollection AddOpenTelemetryRegistration(this IServiceCollection services, IConfiguration configuration)
    {
        var appInsightsConnectionString = configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
        services.AddOpenTelemetryRegistration(appInsightsConnectionString);
        return services;
    }
}
