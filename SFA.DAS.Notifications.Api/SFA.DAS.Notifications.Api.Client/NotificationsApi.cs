using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Notifications.Api.Client.Configuration;
using SFA.DAS.Notifications.Api.Types;

namespace SFA.DAS.Notifications.Api.Client
{
    public class NotificationsApi : HttpClientBase, INotificationsApi
    {
        private readonly INotificationsApiClientConfiguration _configuration;

        public NotificationsApi(INotificationsApiClientConfiguration configuration)
            : base(configuration.ClientToken)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));
            _configuration = configuration;
        }

        public async Task SendEmail(Email email)
        {
            var url = $"{_configuration.BaseUrl}api/email";

            await PostEmail(url, email);
        }

        private async Task PostEmail(string url, Email email)
        {
            var data = JsonConvert.SerializeObject(email);

            await PostAsync(url, data);
        }
    }
}