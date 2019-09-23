using System.Linq;
using SFA.DAS.BadLanguage.Data;

namespace SFA.DAS.BadLanguage.Service
{
    public class BadLanguageService : IBadLanguageService
    {
        private readonly IBadLanguageRepository _badLanguageRepository;

        public BadLanguageService(IBadLanguageRepository badLanguageRepository)
        {
            _badLanguageRepository = badLanguageRepository;
        }

        public bool ContainsBadLanguage(string phrase)
        {
            return _badLanguageRepository.GetBadLanguageList().Any(x => phrase.ToLower().Contains(x.ToLower()));
        }
    }
}
