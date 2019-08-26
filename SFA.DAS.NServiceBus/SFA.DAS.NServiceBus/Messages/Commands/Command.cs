using System;
using NServiceBus;

// ReSharper disable once CheckNamespace - Required for backwards compatibility
namespace SFA.DAS.NServiceBus
{
    [Obsolete("Message conventions are preferred over marker classes/interfaces, consider calling '" + nameof(Configuration.EndpointConfigurationExtensions.UseMessageConventions) + "' endpoint configuration extension instead")]
    public abstract class Command : ICommand
    {
    }
}