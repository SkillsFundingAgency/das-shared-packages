﻿using System;
using System.Collections.Generic;

namespace SFA.DAS.Recruit.Vacancies.Client.Entities
{
    public class LiveVacancy
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public DateTime LastUpdated { get; set; }
        public Guid VacancyId { get; set; }
        public string ApplicationInstructions { get; set; }
        public string ApplicationMethod { get; set; }
        public string ApplicationUrl { get; set; }
        public DateTime ClosingDate { get; set; }
        public string Description { get; set; }
        public string DisabilityConfident { get; set; }
        public string EmployerContactEmail { get; set; }
        public string EmployerContactName { get; set; }
        public string EmployerContactPhone { get; set; }
        public string EmployerDescription { get; set; }
        public Address EmployerLocation { get; set; }
        public string EmployerName { get; set; }
        public string EmployerWebsiteUrl { get; set; }
        public DateTime LiveDate { get; set; }
        public int NumberOfPositions { get; set; }
        public string OutcomeDescription { get; set; }
        public string ProgrammeId { get; set; }
        public string ProgrammeLevel { get; set; }
        public string ProgrammeType { get; set; }
        public IEnumerable<Qualification> Qualifications { get; set; }
        public string ShortDescription { get; set; }
        public IEnumerable<string> Skills { get; set; }
        public DateTime StartDate { get; set; }
        public string ThingsToConsider { get; set; }
        public string Title { get; set; }
        public string TrainingDescription { get; set; }
        public TrainingProvider TrainingProvider { get; set; }
        public long VacancyReference { get; set; }
        public Wage Wage { get; set; }
    }
}
