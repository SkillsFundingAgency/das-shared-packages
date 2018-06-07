using System;
using System.Collections.Generic;

namespace SFA.DAS.Recruit.Vacancies.Client.Entities
{
    public class VacancyApplication
    {
        public Guid CandidateId { get; set; }
        public long VacancyReference { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string AddressLine3 { get; set; }
        public string AddressLine4 { get; set; }
        public DateTime ApplicationDate { get; set; }
        public DateTime BirthDate { get; set; }
        public string DisabilityStatus { get; set; }
        public int EducationFromYear { get; set; }
        public string EducationInstitution { get; set; }
        public int EducationToYear { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string HobbiesAndInterests { get; set; }
        public string Improvements { get; set; }
        public string LastName { get; set; }
        public string Phone { get; set; }
        public string Postcode { get; set; }
        public List<CandidateQualification> Qualifications { get; set; }
        public List<string> Skills { get; set; }
        public string Strengths { get; set; }
        public string Support { get; set; }
        public List<CandidateTrainingCourse> TrainingCourses { get; set; }
        public List<CandidateWorkExperience> WorkExperiences { get; set; }
        
        public class CandidateQualification
        {
            public string QualificationType { get; set; }
            public string Subject { get; set; }
            public string Grade { get; set; }
            public bool IsPredicted { get; set; }
            public int Year { get; set; }
        }

        public class CandidateTrainingCourse
        {
            public string Provider { get; set; }
            public string Title { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
        }

        public class CandidateWorkExperience
        {
            public string Employer { get; set; }
            public string JobTitle { get; set; }
            public string Description { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
        }
    }
}
