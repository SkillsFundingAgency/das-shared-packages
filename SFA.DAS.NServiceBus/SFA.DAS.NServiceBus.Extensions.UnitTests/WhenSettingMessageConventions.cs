using FluentAssertions;
using NUnit.Framework;


namespace SFA.DAS.NServiceBus.Extensions.UnitTests;

public class WhenSettingMessageConventions
{
    [Test]
    public void Then_IsMessage_should_confirm_is_message()
    {
        var data = new Messages.TestMessage();
        EndpointConfigurationExtensions.IsMessage(data.GetType()).Should().BeTrue();
    }

    [Test]
    public void Then_IsMessage_should_confirm_is_not_a_message()
    {
        var data = new object();
        EndpointConfigurationExtensions.IsMessage(data.GetType()).Should().BeFalse();
    }

    [Test]
    public void Then_IsEvent_should_confirm_is_event()
    {
        var data = new Messages.Events.TestEvent();
        EndpointConfigurationExtensions.IsEvent(data.GetType()).Should().BeTrue();
    }

    [Test]
    public void Then_ISEvent_should_confirm_is_not_event()
    {
        var data = new object();
        EndpointConfigurationExtensions.IsEvent(data.GetType()).Should().BeFalse();
    }

    [Test]
    public void Then_IsCommand_should_confirm_is_command()
    {
        var data = new Messages.Commands.TestCommand();
        EndpointConfigurationExtensions.IsCommand(data.GetType()).Should().BeTrue();
    }

    [Test]
    public void Then_IsCommand_should_confirm_is_not_a_command()
    {
        var data = new object();
        EndpointConfigurationExtensions.IsCommand(data.GetType()).Should().BeFalse();
    }
}
