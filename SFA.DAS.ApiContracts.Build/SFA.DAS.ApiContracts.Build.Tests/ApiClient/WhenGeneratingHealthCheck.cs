using System.Net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.Apim.Shared.Infrastructure.HealthCheck;
using SFA.DAS.Apim.Shared.InnerApi.InnerApi.Requests;
using DefaultClient = SFA.DAS.ApiContracts.Build.Tests.Client;
using HealthCheckClient = SFA.DAS.ApiContracts.Build.Tests.HealthCheckFixture.Client;
using OverrideClient = SFA.DAS.ApiContracts.Build.Tests.Override.Client;

namespace SFA.DAS.ApiContracts.Build.Tests.ApiClient;

public class WhenGeneratingHealthCheck
{
    private Mock<HealthCheckClient.ITestApiClient<HealthCheckClient.TestApiConfiguration>> _apiClient = null!;
    private HealthCheckClient.TestApiHealthCheck _sut = null!;

    [SetUp]
    public void SetUp()
    {
        _apiClient = new Mock<HealthCheckClient.ITestApiClient<HealthCheckClient.TestApiConfiguration>>();
        _sut = new HealthCheckClient.TestApiHealthCheck(
            _apiClient.Object,
            Mock.Of<ILogger<HealthCheckClient.TestApiHealthCheck>>());
    }

    [Test]
    public void Then_The_Generated_Class_Uses_The_Generic_Apim_Health_Check()
    {
        _sut.Should().BeAssignableTo<IHealthCheck>();
        _sut.GetType().BaseType.Should()
            .Be(typeof(ApiHealthCheck<HealthCheckClient.TestApiConfiguration>));
    }

    [Test]
    public void Then_No_Health_Check_Is_Generated_By_Default()
    {
        typeof(DefaultClient.TestApiClient).Assembly
            .GetType("SFA.DAS.ApiContracts.Build.Tests.Client.TestApiHealthCheck")
            .Should().BeNull();
    }

    [Test]
    public void Then_The_Configured_Description_Is_Escaped_And_Preserved()
    {
        HealthCheckClient.TestApiHealthCheck.HealthCheckDescription
            .Should().Be("Test \"API\" \\ health");
        HealthCheckClient.TestApiHealthCheck.HealthCheckResultDescription
            .Should().Be("Test \"API\" \\ health check");
    }

    [TestCase(HttpStatusCode.OK, HealthStatus.Healthy)]
    [TestCase(HttpStatusCode.NoContent, HealthStatus.Unhealthy)]
    [TestCase(HttpStatusCode.NotFound, HealthStatus.Unhealthy)]
    [TestCase(HttpStatusCode.InternalServerError, HealthStatus.Unhealthy)]
    public async Task Then_The_Injected_Client_Is_Pinged_And_Health_Is_Returned(
        HttpStatusCode statusCode, HealthStatus expectedStatus)
    {
        _apiClient.Setup(x => x.GetResponseCode(It.IsAny<GetPingRequest>()))
            .ReturnsAsync(statusCode);

        var actual = await _sut.CheckHealthAsync(new HealthCheckContext());

        actual.Status.Should().Be(expectedStatus);
        actual.Description.Should()
            .Be(HealthCheckClient.TestApiHealthCheck.HealthCheckResultDescription);
        _apiClient.Verify(x => x.GetResponseCode(It.IsAny<GetPingRequest>()), Times.Once);
    }

    [Test]
    public async Task Then_The_Generated_Health_Check_Can_Be_Registered_And_Run()
    {
        _apiClient.Setup(x => x.GetResponseCode(It.IsAny<GetPingRequest>()))
            .ReturnsAsync(HttpStatusCode.OK);
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddSingleton(_apiClient.Object);
        services.AddHealthChecks().AddCheck<HealthCheckClient.TestApiHealthCheck>("test-api");
        using var provider = services.BuildServiceProvider();

        var actual = await provider.GetRequiredService<HealthCheckService>().CheckHealthAsync();

        actual.Status.Should().Be(HealthStatus.Healthy);
        actual.Entries["test-api"].Description.Should()
            .Be(HealthCheckClient.TestApiHealthCheck.HealthCheckResultDescription);
    }

    [Test]
    public async Task Then_A_Configuration_Name_Override_Is_Used_And_Description_Defaults_To_Client_Name()
    {
        var apiClient = new Mock<OverrideClient.ITestApiClient<OverrideClient.OverriddenApiConfiguration>>();
        apiClient.Setup(x => x.GetResponseCode(It.IsAny<GetPingRequest>()))
            .ReturnsAsync(HttpStatusCode.OK);
        var _sut = new OverrideClient.TestApiHealthCheck(
            apiClient.Object,
            Mock.Of<ILogger<OverrideClient.TestApiHealthCheck>>());

        var actual = await _sut.CheckHealthAsync(new HealthCheckContext());

        _sut.GetType().BaseType.Should()
            .Be(typeof(ApiHealthCheck<OverrideClient.OverriddenApiConfiguration>));
        actual.Status.Should().Be(HealthStatus.Healthy);
        actual.Description.Should().Be("TestApi check");
        apiClient.Verify(x => x.GetResponseCode(It.IsAny<GetPingRequest>()), Times.Once);
    }
}
