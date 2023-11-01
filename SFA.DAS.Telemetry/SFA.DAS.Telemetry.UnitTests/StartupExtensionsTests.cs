using Microsoft.Extensions.DependencyInjection;
using Moq;
using NUnit.Framework;
using SFA.DAS.Telemetry.RedactionService;
using SFA.DAS.Telemetry.Startup;

namespace SFA.DAS.Telemetry.UnitTests;
public class StartupExtensionsTests
{
    [Test]
    public void AddTelemetryUriRedaction_RegistersUriRedactionServiceWithCorrectKeys()
    {
        Mock<IServiceCollection> serviceMock = new Mock<IServiceCollection>();

        StartupExtensions.AddTelemetryUriRedaction(serviceMock.Object, "firstName, lastName");

        serviceMock.Setup(s => s.Add(It.Is<ServiceDescriptor>(d => d.ServiceType == typeof(IUriRedactionService) && d.ImplementationInstance != null)));
    }
}
