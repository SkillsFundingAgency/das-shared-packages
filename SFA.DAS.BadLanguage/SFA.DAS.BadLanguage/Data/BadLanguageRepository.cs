using System.Collections.Generic;

namespace SFA.DAS.BadLanguage.Data
{
    public class BadLanguageRepository : IBadLanguageRepository
    {
        public List<string> GetBadLanguageList()
        {
            throw new System.NotImplementedException("Data Layer Pending Architectural Decision");
        }
    }
}