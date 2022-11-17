using NServiceBus;

namespace SFA.DAS.NServiceBus.AzureFunction.Extensions;

public static class SendLocally
{
    public static SendOptions Options
    {
        get
        {
            var options = new SendOptions();
            options.RouteToThisEndpoint();
            return options;
        }
    }
}