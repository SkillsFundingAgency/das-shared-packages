using System;

namespace SFA.DAS.BadLanguage
{
    public interface IBadLanguageService
    {
        bool ContainsBadLanguage(string phrase);
    }
}
