using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Events.Api.Client.Configuration;
using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.Events.Api.Client
{
    public partial class EventsApi :  IEventsApi
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

        public EventsApi(IEventsApiClientConfiguration configuration) : this(new SecureHttpClient(), configuration) { }

        /// <summary>
        /// Creates a new ApprenticeshipEvent
        /// </summary>
        /// <param name="apprenticeshipEvent">ApprenticeshipEvent to create</param>
        /// <returns></returns>
        public async Task CreateApprenticeshipEvent(ApprenticeshipEvent apprenticeshipEvent)
        {
            var url = $"{_configuration.BaseUrl}api/events/apprenticeships";

            await PostApprenticeshipEvent(url, apprenticeshipEvent);
        }

        /// <summary>
        /// Get a list of ApprenticeshipEvents starting from the supplied Id
        /// </summary>
        /// <param name="fromEventId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns>List of ApprenticeshipEvents</returns>
        public async Task<List<ApprenticeshipEventView>> GetApprenticeshipEventsById(long fromEventId = 0, int pageSize = 1000, int pageNumber = 1)
        {
            var url = $"{_configuration.BaseUrl}api/events/apprenticeships?fromEventId={fromEventId}&pageSize={pageSize}&pageNumber={pageNumber}";

            return await GetApprenticeshipEvents(url);
        }

        /// <summary>
        /// Get a list of ApprenticeshipEvents by date range
        /// </summary>
        /// <param name="fromDate">If not supplied, will revert to start of time</param>
        /// <param name="toDate">If not supplied, will revert to end of time</param>
        /// <param name="pageSize">Maximum of 10,000</param>
        /// <param name="pageNumber"></param>
        /// <returns>List of ApprenticeshipEvents</returns>
        public async Task<List<ApprenticeshipEventView>> GetApprenticeshipEventsByDateRange(DateTime? fromDate = null, DateTime? toDate = null, int pageSize = 1000, int pageNumber = 1)
        {
            var dateString = BuildDateQuery(fromDate, toDate);

            var url = $"{_configuration.BaseUrl}api/events/apprenticeships?{dateString}pageSize={pageSize}&pageNumber={pageNumber}";

            return await GetApprenticeshipEvents(url);
        }

        private async Task PostApprenticeshipEvent(string url, ApprenticeshipEvent apprenticeshipEvent)
        {
            var data = JsonConvert.SerializeObject(apprenticeshipEvent);

            await _secureHttpClient.PostAsync(url, data, _configuration.ClientToken);
        }

        private async Task<List<ApprenticeshipEventView>> GetApprenticeshipEvents(string url)
        {
            var content = await _secureHttpClient.GetAsync(url, _configuration.ClientToken);

            return JsonConvert.DeserializeObject<List<ApprenticeshipEventView>>(content);
        }

        private static string BuildDateQuery(DateTime? fromDate, DateTime? toDate)
        {
            var fromDateString = FormatDateTime(fromDate);
            var toDateString = FormatDateTime(toDate);

            if (string.IsNullOrWhiteSpace(fromDateString))
                return string.IsNullOrWhiteSpace(toDateString) ? string.Empty : $"toDate={toDateString}&";

            return string.IsNullOrWhiteSpace(toDateString) ? $"fromDate={fromDateString}&" : $"fromDate={fromDateString}&toDate={toDateString}&";
        }

        private static string FormatDateTime(DateTime? source)
        {
            return source.HasValue ? $"{source:yyyyMMddHHmmss}" : string.Empty;
        }
    }
}
