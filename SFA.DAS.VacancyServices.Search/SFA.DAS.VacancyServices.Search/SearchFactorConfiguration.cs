namespace SFA.DAS.VacancyServices.Search
{
    internal class SearchFactorConfiguration
    {
        public SearchTermFactors JobTitleFactors { get; set; }

        public SearchTermFactors EmployerFactors { get; set; }

        public SearchTermFactors DescriptionFactors { get; set; }
    }

    internal class SearchTermFactors
    {
        public double? Boost { get; set; }

        public int? Fuzziness { get; set; }

        public int? FuzzyPrefix { get; set; }

        public bool MatchAllKeywords { get; set; }

        public int? PhraseProximity { get; set; }

        public string MinimumMatch { get; set; }
    }
}
