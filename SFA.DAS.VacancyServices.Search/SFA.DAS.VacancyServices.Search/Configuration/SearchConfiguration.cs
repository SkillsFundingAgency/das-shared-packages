using System.Collections.Generic;

namespace SFA.DAS.VacancyServices.Search.Configuration
{

    public class SearchConfiguration
    {
        public SearchConfiguration()
        {
            Synonyms = new List<string>();
            ExcludedTerms = new List<string>();
        }

        public string HostName { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }

        public int NodeCount { get; set; }

        public int Timeout { get; set; }

        public IEnumerable<string> Synonyms { get; set; }

        public IEnumerable<string> ExcludedTerms { get; set; }

        public IEnumerable<string> StopwordsBase { get; set; }

        public IEnumerable<string> StopwordsExtended { get; set; }

        public string ApprenticeshipsIndex { get; set; }

        public string TraineeshipsIndex { get; set; }

        public string LocationsIndex { get; set; }
    }
}
