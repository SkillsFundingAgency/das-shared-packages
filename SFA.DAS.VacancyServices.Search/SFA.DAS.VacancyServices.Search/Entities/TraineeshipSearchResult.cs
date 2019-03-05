namespace SFA.DAS.VacancyServices.Search.Entities
{
    using System;

    public class TraineeshipSearchResult
    {
        public int Id { get; set; }
        public string AnonymousEmployerName { get; set; }
        public string Category { get; set; }
        public string CategoryCode { get; set; }
        public DateTime ClosingDate { get; set; }
        public string Description { get; set; }
        public string EmployerName { get; set; }
        public bool IsDisabilityConfident { get; set; }
        public bool IsEmployerAnonymous { get; set; }
        public bool IsPositiveAboutDisability { get; set; }
        public GeoPoint Location { get; set; }
        public int NumberOfPositions { get; set; }
        public DateTime PostedDate { get; set; }
        public string ProviderName { get; set; }
        public DateTime StartDate { get; set; }
        public string SubCategory { get; set; }
        public string SubCategoryCode { get; set; }
        public string Title { get; set; }
        public long Ukprn { get; set; }
        public string VacancyReference { get; set; }

        //Calculated after search
        public double Distance { get; set; }
        public double Score { get; set; }
    }
}