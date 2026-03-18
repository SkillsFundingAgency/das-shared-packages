using System.Threading.Tasks;
using NServiceBus;
using SFA.DAS.Notifications.Messages.Commands;
using SFA.DAS.SharedOuterApi.Interfaces;

namespace SFA.DAS.SharedOuterApi.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IMessageSession _messageSession;

        public NotificationService(IMessageSession messageSession)
        {
            _messageSession = messageSession;
        }

        public Task Send(SendEmailCommand email)
        {
            return _messageSession.Send(email);
        }
    }
}