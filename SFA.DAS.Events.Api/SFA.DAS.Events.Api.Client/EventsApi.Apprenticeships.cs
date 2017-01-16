using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.Events.Api.Client
{
    public partial class EventsApi :  IEventsApi
    {
        /// <summary>
        /// Creates a new ApprenticeshipEvent
        /// </summary>
        /// <param name="apprenticeshipEvent">ApprenticeshipEvent to create</param>
        /// <returns></returns>
        public async Task CreateApprenticeshipEvent(ApprenticeshipEvent apprenticeshipEvent)
        {
            var url = $"{_configuration.BaseUrl}api/events/apprenticeships";

            await PostEvent(url, apprenticeshipEvent);
        }

        /// <summary>
        /// Creates a number of ApprenticeshipEvents
        /// </summary>
        /// <param name="apprenticeshipEvents">ApprenticeshipEvents to create</param>
        /// <returns></returns>
        public async Task BulkCreateApprenticeshipEvent(IList<ApprenticeshipEvent> apprenticeshipEvents)
        {
            var url = $"{_configuration.BaseUrl}api/events/apprenticeships/bulk";

            await PostEvent(url, apprenticeshipEvents);
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

            return await GetEvents<ApprenticeshipEventView>(url);
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

            return await GetEvents<ApprenticeshipEventView>(url);
        }
    }
}
