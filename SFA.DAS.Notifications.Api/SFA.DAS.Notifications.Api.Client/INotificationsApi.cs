using System.Threading.Tasks;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.Notifications.Api.Client
{
    public interface INotificationsApi
    {
        Task SendEmail(Email email);
    }
}