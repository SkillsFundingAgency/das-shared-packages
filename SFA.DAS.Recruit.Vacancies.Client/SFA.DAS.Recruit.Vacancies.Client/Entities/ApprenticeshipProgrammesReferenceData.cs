using System;
using System.Collections.Generic;

namespace SFA.DAS.Recruit.Vacancies.Client.Entities
{
    public class ApprenticeshipProgrammesReferenceData
    {
        public string Id { get; set; }
        public DateTime LastUpdatedDate { get; set; } 
        public List<ApprenticeshipProgramme> Data { get; set; }
    }
}