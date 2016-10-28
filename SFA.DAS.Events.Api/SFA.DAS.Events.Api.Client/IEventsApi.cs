using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.Events.Api.Client
{
    public interface IEventsApi
    {
        Task CreateApprenticeshipEvent(ApprenticeshipEvent apprenticeshipEvent);
        Task<List<ApprenticeshipEventView>> GetApprenticeshipEventsById(long fromEventId = 0, int pageSize = 1000, int pageNumber = 1);
        Task<List<ApprenticeshipEventView>> GetApprenticeshipEventsByDateRange(DateTime? fromDate = null, DateTime? toDate = null, int pageSize = 1000, int pageNumber = 1);

        Task CreateAgreementEvent(AgreementEvent agreementEvent);
        Task<List<AgreementEventView>> GetAgreementEventsById(long fromEventId = 0, int pageSize = 1000, int pageNumber = 1);
        Task<List<AgreementEventView>> GetAgreementEventsByDateRange(DateTime? fromDate = null, DateTime? toDate = null, int pageSize = 1000, int pageNumber = 1);
    }
}
