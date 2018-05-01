namespace SFA.DAS.Recruit.Vacancies.Client.Entities
{
    public class Wage
    {
        public int Duration { get; set; }

        public string DurationUnit { get; set; }

        public string WorkingWeekDescription { get; set; }

        public decimal WeeklyHours { get; set; }

        public string WageType { get; set; }

        public decimal? FixedWageYearlyAmount { get; set; }

        public string WageAdditionalInformation { get; set; }
    }
}
