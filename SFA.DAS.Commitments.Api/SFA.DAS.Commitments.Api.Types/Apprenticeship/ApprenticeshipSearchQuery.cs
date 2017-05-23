using System.Collections.Generic;

using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;

namespace SFA.DAS.Commitments.Api.Types.Apprenticeship
{
    public sealed class ApprenticeshipSearchQuery
    {
        public List<ApprenticeshipStatus> ApprenticeshipStatuses { get; set; }

        public List<RecordStatus> RecordStatuses { get; set; }

        public List<long> TrainingProviderIds { get; set; }

        public List<string> EmployerOrganisationIds { get; set; }

        public List<string> TrainingCourses { get; set; }
    }
}