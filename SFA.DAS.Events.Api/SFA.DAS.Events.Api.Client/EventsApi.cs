using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Events.Api.Client.Configuration;

namespace SFA.DAS.Events.Api.Client
{
    public partial class EventsApi
    {
        private readonly IEventsApiClientConfiguration _configuration;
        private readonly ISecureHttpClient _secureHttpClient;

        public EventsApi(ISecureHttpClient secureHttpClient, IEventsApiClientConfiguration configuration)
        {
            if (secureHttpClient == null)
                throw new ArgumentNullException(nameof(secureHttpClient));

            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            _secureHttpClient = secureHttpClient;
            _configuration = configuration;
        }

        public EventsApi(IEventsApiClientConfiguration configuration) : this(new SecureHttpClient(), configuration)
        {
        }

        private async Task PostEvent<T>(string url, T @event)
        {
            var data = JsonConvert.SerializeObject(@event);

            await _secureHttpClient.PostAsync(url, data, _configuration.ClientToken);
        }

        private async Task<List<T>> GetEvents<T>(string url)
        {
            var content = await _secureHttpClient.GetAsync(url, _configuration.ClientToken);

            return JsonConvert.DeserializeObject<List<T>>(content);
        }

        private static string BuildDateQuery(DateTime? fromDate, DateTime? toDate)
        {
            var fromDateString = FormatDateTime(fromDate);
            var toDateString = FormatDateTime(toDate);

            if (string.IsNullOrWhiteSpace(fromDateString))
                return string.IsNullOrWhiteSpace(toDateString) ? string.Empty : $"toDate={toDateString}&";

            return string.IsNullOrWhiteSpace(toDateString)
                ? $"fromDate={fromDateString}&"
                : $"fromDate={fromDateString}&toDate={toDateString}&";
        }

        private static string FormatDateTime(DateTime? source)
        {
            return source.HasValue ? $"{source:yyyyMMddHHmmss}" : string.Empty;
        }
    }
}
