using System.Collections.Generic;

using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;

namespace SFA.DAS.Commitments.Api.Types.Apprenticeship
{
    public sealed class Facets
    {
        public List<FacetItem<ApprenticeshipStatus>> ApprenticeshipStatuses { get; set; }

        public List<FacetItem<RecordStatus>> RecordStatuses { get; set; }

        public List<FacetItem<User>> TrainingProviders { get; set; }

        public List<FacetItem<User>> EmployerOrganisations { get; set; }

        public List<FacetItem<TrainingCourse>> TrainingCourses { get; set; }
    }
}