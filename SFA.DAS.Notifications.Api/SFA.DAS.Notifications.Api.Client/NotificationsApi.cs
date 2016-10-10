using SFA.DAS.Notifications.Api.Client.Configuration;

namespace SFA.DAS.Notifications.Api.Client
{
    public class NotificationsApi : HttpClientBase, INotificationsApi
    {
        public NotificationsApi(INotificationsApiClientConfiguration configuration)
            : base(configuration.ClientToken)
        {
        }


    }
}