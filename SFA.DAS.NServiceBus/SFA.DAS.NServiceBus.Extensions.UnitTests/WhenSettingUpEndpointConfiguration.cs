using FluentAssertions;
using NServiceBus.Configuration.AdvancedExtensibility;
using NUnit.Framework;
using SFA.DAS.NServiceBus.Extensions.UnitTests.Messages.Commands;
using SFA.DAS.NServiceBus.Extensions.UnitTests.Messages.Events;

namespace SFA.DAS.NServiceBus.Extensions.UnitTests;

public class WhenSettingUpEndpointConfiguration
{
    [Test]
    public void Then_UseSerializer_does_not_throw_error()
    {
        var sut = new EndpointConfiguration("test");
        sut.UseNewtonsoftJsonSerializer();
        sut.GetSettings().Get<object>("MainSerializer").Should().NotBeNull();
    }

    [Test]
    public void Then_UseMessageConventions_does_set_the_event_command_types()
    {
        var sut = new EndpointConfiguration("test");
        sut.UseMessageConventions();
        var conventions = sut.Conventions();
        conventions.Conventions.IsCommandType(typeof(TestCommand)).Should().BeTrue();
        conventions.Conventions.IsEventType(typeof(TestEvent)).Should().BeTrue();
    }
}
