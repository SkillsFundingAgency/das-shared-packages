using System.Collections.Generic;

using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;

namespace SFA.DAS.Commitments.Api.Types.Apprenticeship
{
    public sealed class ApprenticeshipSearchQuery
    {
        public ApprenticeshipSearchQuery()
        {
            PageNumber = 1;
            PageSize = 25;
        }

        public List<ApprenticeshipStatus> ApprenticeshipStatuses { get; set; }

        public List<RecordStatus> RecordStatuses { get; set; }

        public List<long> TrainingProviderIds { get; set; }

        public List<string> EmployerOrganisationIds { get; set; }

        public List<string> TrainingCourses { get; set; }

        public int PageNumber { get; set; }

        /// <summary>
        /// Number of results per page. Default: 25
        /// </summary>
        public int PageSize { get; set; }
    }
}