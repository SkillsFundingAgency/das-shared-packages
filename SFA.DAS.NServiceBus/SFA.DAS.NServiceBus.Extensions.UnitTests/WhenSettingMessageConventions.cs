using AutoFixture.NUnit3;
using FluentAssertions;
using NUnit.Framework;


namespace SFA.DAS.NServiceBus.Extensions.UnitTests;

public class WhenSettingMessageConventions
{
    [Test, AutoData]
    public void Then_IsMessage_should_confirm_is_message(Messages.TestMessage data)
    {
        EndpointConfigurationExtensions.IsMessage(data.GetType()).Should().BeTrue();
    }

    [Test, AutoData]
    public void Then_IsMessage_should_confirm_is_not_a_message(object data)
    {
        EndpointConfigurationExtensions.IsMessage(data.GetType()).Should().BeFalse();
    }

    [Test, AutoData]
    public void Then_IsEvent_should_confirm_is_event(Messages.Events.TestEvent data)
    {
        EndpointConfigurationExtensions.IsEvent(data.GetType()).Should().BeTrue();
    }

    [Test, AutoData]
    public void Then_ISEvent_should_confirm_is_not_event(object data)
    {
        EndpointConfigurationExtensions.IsEvent(data.GetType()).Should().BeFalse();
    }

    [Test, AutoData]
    public void Then_IsCommand_should_confirm_is_command(Messages.Commands.TestCommand data)
    {
        EndpointConfigurationExtensions.IsCommand(data.GetType()).Should().BeTrue();
    }

    [Test, AutoData]
    public void Then_IsCommand_should_confirm_is_not_a_command(object data)
    {
        EndpointConfigurationExtensions.IsCommand(data.GetType()).Should().BeFalse();
    }
}
