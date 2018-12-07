using System;
using NServiceBus;

namespace SFA.DAS.NServiceBus
{
    [Obsolete("Message conventions are preferred over marker classes/interfaces, consider calling '" + nameof(EndpointConfigurationExtensions.UseMessageConventions) + "' endpoint configuration extension instead")]
    public abstract class Command : ICommand
    {
    }
}