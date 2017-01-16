using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.Events.Api.Client
{
    public partial class EventsApi
    {
        /// <summary>
        /// Creates a new AccountEvent
        /// </summary>
        /// <param name="accountEvent">AccountEvent to create</param>
        /// <returns></returns>
        public async Task CreateAccountEvent(AccountEvent accountEvent)
        {
            var url = $"{_configuration.BaseUrl}api/events/accounts";

            await PostAccountEvent(url, accountEvent);
        }

        /// <summary>
        /// Get a list of AccountEvents starting from the supplied Id
        /// </summary>
        /// <param name="fromEventId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns>List of AccountEvents</returns>
        public async Task<List<AccountEventView>> GetAccountEventsById(long fromEventId = 0, int pageSize = 1000, int pageNumber = 1)
        {
            var url = $"{_configuration.BaseUrl}api/events/accounts?fromEventId={fromEventId}&pageSize={pageSize}&pageNumber={pageNumber}";

            return await GetAccountEvents(url);
        }

        /// <summary>
        /// Get a list of AccountEvents by date range
        /// </summary>
        /// <param name="fromDate">If not supplied, will revert to start of time</param>
        /// <param name="toDate">If not supplied, will revert to end of time</param>
        /// <param name="pageSize">Maximum of 10,000</param>
        /// <param name="pageNumber"></param>
        /// <returns>List of AccountEvents</returns>
        public async Task<List<AccountEventView>> GetAccountEventsByDateRange(DateTime? fromDate = null, DateTime? toDate = null, int pageSize = 1000, int pageNumber = 1)
        {
            var dateString = BuildDateQuery(fromDate, toDate);

            var url = $"{_configuration.BaseUrl}api/events/accounts?{dateString}pageSize={pageSize}&pageNumber={pageNumber}";

            return await GetAccountEvents(url);
        }

        private async Task PostAccountEvent(string url, AccountEvent accountEvent)
        {
            var data = JsonConvert.SerializeObject(accountEvent);

            await _secureHttpClient.PostAsync(url, data, _configuration.ClientToken);
        }

        private async Task<List<AccountEventView>> GetAccountEvents(string url)
        {
            var content = await _secureHttpClient.GetAsync(url, _configuration.ClientToken);

            return JsonConvert.DeserializeObject<List<AccountEventView>>(content);
        }
    }
}
