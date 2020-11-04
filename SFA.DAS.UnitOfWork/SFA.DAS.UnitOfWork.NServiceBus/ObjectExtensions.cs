using System.Text.RegularExpressions;
using NServiceBus;

namespace SFA.DAS.UnitOfWork.NServiceBus
{
    internal static class ObjectExtensions
    {
        internal static bool IsCommand(this object message)
        {
            return Regex.IsMatch(message.GetType().Name, "Command(V\\d+)?$") || message is ICommand;
        }
    }
}
