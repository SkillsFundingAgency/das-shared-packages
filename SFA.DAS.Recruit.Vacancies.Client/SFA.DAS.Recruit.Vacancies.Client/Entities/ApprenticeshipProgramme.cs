using System;

namespace SFA.DAS.Recruit.Vacancies.Client.Entities
{
    public class ApprenticeshipProgramme
    {
        public string Id { get; set; }

        public TrainingType ApprenticeshipType { get; set; }

        public string Title { get; set; }

        public DateTime? EffectiveFrom { get; set; }

        public DateTime? EffectiveTo { get; set; }

        public DateTime? LastDateStarts { get; set; }

        public ApprenticeshipLevel ApprenticeshipLevel { get; set; }

        public int Duration { get; set; }

        public bool IsActive { get; set; }

        public int? EducationLevelNumber { get; set; }
    }
}