namespace SFA.DAS.Recruit.Vacancies.Client.Entities
{
    public class ApplicationQualification
    {
        public string QualificationType { get; set; }
        public string Subject { get; set; }
        public string Grade { get; set; }
        public bool IsPredicted { get; set; }
        public int Year { get; set; }
    }
}