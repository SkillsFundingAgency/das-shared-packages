using System.Threading.Tasks;
using SFA.DAS.Notifications.Messages.Commands;

namespace SFA.DAS.Apim.Shared.Interfaces
{
    public interface INotificationService
    {
        Task Send(SendEmailCommand email);
    }
}