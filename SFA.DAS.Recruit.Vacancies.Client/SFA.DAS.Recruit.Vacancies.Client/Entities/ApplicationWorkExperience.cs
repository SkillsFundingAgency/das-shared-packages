using System;

namespace SFA.DAS.Recruit.Vacancies.Client.Entities
{
    public class ApplicationWorkExperience
    {
        public string Employer { get; set; }
        public string JobTitle { get; set; }
        public string Description { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
