using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.Events.Api.Client
{
    public interface IEventsApi
    {
        Task CreateApprenticeshipEvent(ApprenticeshipEvent apprenticeshipEvent);
        Task BulkCreateApprenticeshipEvent(IList<ApprenticeshipEvent> apprenticeshipEvents);
        Task<List<ApprenticeshipEventView>> GetApprenticeshipEventsById(long fromEventId = 0, int pageSize = 1000, int pageNumber = 1);
        Task<List<ApprenticeshipEventView>> GetApprenticeshipEventsByDateRange(DateTime? fromDate = null, DateTime? toDate = null, int pageSize = 1000, int pageNumber = 1);

        Task CreateAgreementEvent(AgreementEvent agreementEvent);
        Task<List<AgreementEventView>> GetAgreementEventsById(long fromEventId = 0, int pageSize = 1000, int pageNumber = 1);
        Task<List<AgreementEventView>> GetAgreementEventsByDateRange(DateTime? fromDate = null, DateTime? toDate = null, int pageSize = 1000, int pageNumber = 1);

        Task CreateAccountEvent(AccountEvent accountEvent);
        Task<List<AccountEventView>> GetAccountEventsById(long fromEventId = 0, int pageSize = 1000, int pageNumber = 1);
        Task<List<AccountEventView>> GetAccountEventsByDateRange(DateTime? fromDate = null, DateTime? toDate = null, int pageSize = 1000, int pageNumber = 1);

        Task CreateGenericEvent(GenericEvent genericEvent);
        Task<List<GenericEvent>> GetGenericEventsById(string eventType, long fromEventId = 0, int pageSize = 1000, int pageNumber = 1);
        Task<List<GenericEvent>> GetGenericEventsByDateRange(string eventType, DateTime? fromDate = null, DateTime? toDate = null, int pageSize = 1000, int pageNumber = 1);

        Task<List<GenericEvent>> GetGenericEventsByResourceId(string resourceType, string resourceId, DateTime? fromDate = null, DateTime? toDate = null, int pageSize = 1000, int pageNumber = 1);
        Task<List<GenericEvent>> GetGenericEventsByResourceUri(string resourceUri, DateTime? fromDate = null, DateTime? toDate = null, int pageSize = 1000, int pageNumber = 1);

        /// <summary>
        /// Creates a new Generic Event
        /// </summary>
        /// <typeparam name="T">The type of the payload</typeparam>
        /// <param name="payLoad">The body of the generic event</param>
        /// <returns></returns>
        Task CreateGenericEvent<T>(T payLoad);

        /// <summary>
        /// Get a list of GenericEvents starting from the supplied Id
        /// </summary>
        /// <typeparam name="T">the type of the payload</typeparam>
        /// <param name="fromEventId"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        Task<List<GenericEvent<T>>> GetGenericEventsById<T>(long fromEventId = 0, int pageSize = 1000, int pageNumber = 1);

        /// <summary>
        /// Get a list of GenericEvent by date range
        /// </summary>
        /// <typeparam name="T">the type of the payload</typeparam>
        /// <param name="fromDate">If not supplied, will revert to start of time</param>
        /// <param name="toDate">If not supplied, will revert to end of time</param>
        /// <param name="pageSize">Maximum of 10,000</param>
        /// <param name="pageNumber"></param>
        /// <returns></returns>
        Task<List<GenericEvent<T>>> GetGenericEventsByDateRange<T>(DateTime? fromDate = null, DateTime? toDate = null, int pageSize = 1000, int pageNumber = 1);

        Task<List<GenericEvent<T>>> GetGenericEventsByResourceId<T>(string resourceType, string resourceId, DateTime? fromDate = null, DateTime? toDate = null, int pageSize = 1000, int pageNumber = 1);
        Task<List<GenericEvent<T>>> GetGenericEventsByResourceUri<T>(string resourceUri, DateTime? fromDate = null, DateTime? toDate = null, int pageSize = 1000, int pageNumber = 1);
    }
}
