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
        /// <summary>
        /// Creates a new Generic Event
        /// </summary>
        /// <typeparam name="T">The type of the payload</typeparam>
        /// <param name="event">The generic event to create</param>
        /// <returns></returns>
        Task CreateGenericEvent<T>(IGenericEvent<T> @event);
    }
}
