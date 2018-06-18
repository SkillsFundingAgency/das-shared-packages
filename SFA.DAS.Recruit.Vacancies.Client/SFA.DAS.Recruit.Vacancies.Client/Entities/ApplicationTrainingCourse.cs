using System;

namespace SFA.DAS.Recruit.Vacancies.Client.Entities
{
    public class ApplicationTrainingCourse
    {
        public string Provider { get; set; }
        public string Title { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
    }
}
