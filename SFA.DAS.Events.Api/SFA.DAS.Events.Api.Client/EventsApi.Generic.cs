using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Events.Api.Types;
using Newtonsoft.Json;

namespace SFA.DAS.Events.Api.Client
{
    public partial class EventsApi
    {
        /// <summary>
        /// Creates a new Generic Event
        /// </summary>
        /// <param name="genericEvent">GenericEvent to create</param>
        /// <returns></returns>
        public async Task CreateGenericEvent(GenericEvent genericEvent)
        {
            var url = $"{_configuration.BaseUrl}api/events/create";

            await PostEvent(url, genericEvent);
        }

        /// <summary>
        /// Creates a new Generic Event
        /// </summary>
        /// <typeparam name="T">The type of the payload</typeparam>
        /// <param name="payLoad">The body of the generic event</param>
        /// <returns></returns>
        public async Task CreateGenericEvent<T>(T payLoad)
        {
            var @event = new GenericEvent<T>
            {
                Payload = payLoad
            };
            var genericEvent = GenericEventMapper.FromTyped(@event);
            await CreateGenericEvent(genericEvent);
        }

        /// <summary>
        /// Get a list of GenericEvents starting from the supplied Id
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="fromEventId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns>List of GenericEvent</returns>
        public async Task<List<GenericEvent>> GetGenericEventsById(string eventType, long fromEventId = 0, int pageSize = 1000, int pageNumber = 1)
        {
            var url = $"{_configuration.BaseUrl}api/events/getSinceEvent?eventType={eventType}&fromEventId={fromEventId}&pageSize={pageSize}&pageNumber={pageNumber}";

            return await GetEvents<GenericEvent>(url);
        }

        /// <summary>
        /// Get a list of GenericEvents starting from the supplied Id
        /// </summary>
        /// <typeparam name="T">the type of the payload</typeparam>
        /// <param name="fromEventId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public async Task<List<GenericEvent<T>>> GetGenericEventsById<T>(long fromEventId = 0, int pageSize = 1000,
            int pageNumber = 1)
        {
            var list = new List<GenericEvent<T>>();
            var events = await GetGenericEventsById(typeof(T).FullName, fromEventId, pageSize, pageNumber);
            foreach (GenericEvent genericEvent in events)
            {
                var @event = await Task.Factory.StartNew(() => GenericEventMapper.ToTyped<T>(genericEvent));
                list.Add(@event);
            }

            return list;
        }

        /// <summary>
        /// Get a list of GenericEvent by date range
        /// </summary>
        /// <param name="eventType"></param>
        /// <param name="fromDate">If not supplied, will revert to start of time</param>
        /// <param name="toDate">If not supplied, will revert to end of time</param>
        /// <param name="pageSize">Maximum of 10,000</param>
        /// <param name="pageNumber"></param>
        /// <returns>List of GenericEvent</returns>
        public async Task<List<GenericEvent>> GetGenericEventsByDateRange(string eventType, DateTime? fromDate = null, DateTime? toDate = null, int pageSize = 1000, int pageNumber = 1)
        {
            var dateString = BuildDateQuery(fromDate, toDate);

            var url = $"{_configuration.BaseUrl}api/events/getByDateRange?eventType={eventType}&{dateString}pageSize={pageSize}&pageNumber={pageNumber}";

            return await GetEvents<GenericEvent>(url);
        }

        /// <summary>
        /// Get a list of GenericEvent by date range
        /// </summary>
        /// <typeparam name="T">the type of the payload</typeparam>
        /// <param name="fromDate">If not supplied, will revert to start of time</param>
        /// <param name="toDate">If not supplied, will revert to end of time</param>
        /// <param name="pageSize">Maximum of 10,000</param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        public async Task<List<GenericEvent<T>>> GetGenericEventsByDateRange<T>(DateTime? fromDate = null, DateTime? toDate = null, int pageSize = 1000, int pageNumber = 1)
        {
            var list = new List<GenericEvent<T>>();
            var genericEvents = await GetGenericEventsByDateRange(typeof(T).FullName, fromDate, toDate, pageSize, pageNumber);
            foreach (var genericEvent in genericEvents)
            {
                var @event = await Task.Factory.StartNew(() => GenericEventMapper.ToTyped<T>(genericEvent));
                list.Add(@event);
            }

            return list;
        }
    }
}
