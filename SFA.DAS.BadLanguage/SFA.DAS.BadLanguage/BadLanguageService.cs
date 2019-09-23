using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.BadLanguage
{
    public class BadLanguageService : IBadLanguageService
    {
        public bool ContainsBadLanguage(string phrase)
        {
            throw new NotImplementedException();
        }
    }
}
