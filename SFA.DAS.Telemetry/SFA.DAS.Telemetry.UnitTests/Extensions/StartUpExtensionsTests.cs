using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using OpenTelemetry.Trace;
using SFA.DAS.Telemetry.Extensions;
using SFA.DAS.Telemetry.RedactionProcessor;

namespace SFA.DAS.Telemetry.UnitTests.Extensions;

public class StartUpExtensionsTests
{
    [Test]
    public void AddOpenTelemetry_ReturnsTheOriginalServiceCollection()
    {
        ServiceCollection services = new();

        IServiceCollection result = services.AddOpenTelemetry(
            () => new TelemetryOptions());

        Assert.That(result, Is.SameAs(services));
    }

    [Test]
    public void AddOpenTelemetry_RegistersHttpContextAccessor()
    {
        ServiceCollection services = new();

        services.AddOpenTelemetry(
            () => new TelemetryOptions());

        using ServiceProvider serviceProvider = services.BuildServiceProvider();

        Assert.That(
            serviceProvider.GetService<IHttpContextAccessor>(),
            Is.Not.Null);
    }

    [Test]
    public void AddOpenTelemetry_WithNotFoundResponseProcessingEnabled_BuildsTracerProvider()
    {
        ServiceCollection services = new();

        services.AddOpenTelemetry(() => new TelemetryOptions(true, false));

        using ServiceProvider serviceProvider = services.BuildServiceProvider();

        Assert.That(
            serviceProvider.GetRequiredService<TracerProvider>(),
            Is.Not.Null);
    }

    [Test]
    public void AddOpenTelemetry_WithUriRedactionEnabled_BuildsTracerProvider()
    {
        ServiceCollection services = new();

        services.AddOpenTelemetry(
            () => new TelemetryOptions
            {
                UriRedactionOptions = new UriRedactionOptions
                {
                    EnableUriRedaction = true,
                    RedactionList = ["email"]
                }
            });

        using ServiceProvider serviceProvider = services.BuildServiceProvider();

        Assert.That(
            serviceProvider.GetRequiredService<TracerProvider>(),
            Is.Not.Null);
    }

    [Test]
    public void AddOpenTelemetry_WithBothProcessorsEnabled_BuildsTracerProvider()
    {
        ServiceCollection services = new();

        services.AddOpenTelemetry(
            () => new TelemetryOptions
            {
                EnableNotFoundAsSuccessResponse = true,
                UriRedactionOptions = new UriRedactionOptions
                {
                    EnableUriRedaction = true,
                    RedactionList = ["email", "dateOfBirth"]
                }
            });

        using ServiceProvider serviceProvider = services.BuildServiceProvider();

        Assert.That(
            serviceProvider.GetRequiredService<TracerProvider>(),
            Is.Not.Null);
    }
}
