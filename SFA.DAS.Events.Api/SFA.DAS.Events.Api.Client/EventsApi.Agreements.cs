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
        /// Creates a new AgreementEvent
        /// </summary>
        /// <param name="agreementEvent">AgreementEvent to create</param>
        /// <returns></returns>
        public async Task CreateAgreementEvent(AgreementEvent agreementEvent)
        {
            var url = $"{_configuration.BaseUrl}api/events/agreements";

            await PostAgreementEvent(url, agreementEvent);
        }

        /// <summary>
        /// Get a list of AgreementEvents starting from the supplied Id
        /// </summary>
        /// <param name="fromEventId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns>List of AgreementEvents</returns>
        public async Task<List<AgreementEventView>> GetAgreementEventsById(long fromEventId = 0, int pageSize = 1000, int pageNumber = 1)
        {
            var url = $"{_configuration.BaseUrl}api/events/agreements?fromEventId={fromEventId}&pageSize={pageSize}&pageNumber={pageNumber}";

            return await GetAgreementEvents(url);
        }

        /// <summary>
        /// Get a list of AgreementEvents by date range
        /// </summary>
        /// <param name="fromDate">If not supplied, will revert to start of time</param>
        /// <param name="toDate">If not supplied, will revert to end of time</param>
        /// <param name="pageSize">Maximum of 10,000</param>
        /// <param name="pageNumber"></param>
        /// <returns>List of AgreementEvents</returns>
        public async Task<List<AgreementEventView>> GetAgreementEventsByDateRange(DateTime? fromDate = null, DateTime? toDate = null, int pageSize = 1000, int pageNumber = 1)
        {
            var dateString = BuildDateQuery(fromDate, toDate);

            var url = $"{_configuration.BaseUrl}api/events/agreements?{dateString}pageSize={pageSize}&pageNumber={pageNumber}";

            return await GetAgreementEvents(url);
        }

        private async Task PostAgreementEvent(string url, AgreementEvent agreementEvent)
        {
            var data = JsonConvert.SerializeObject(agreementEvent);

            await PostAsync(url, data);
        }

        private async Task<List<AgreementEventView>> GetAgreementEvents(string url)
        {
            var content = await GetAsync(url);

            return JsonConvert.DeserializeObject<List<AgreementEventView>>(content);
        }
    }
}
