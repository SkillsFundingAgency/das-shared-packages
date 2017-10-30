

using System.Collections.Generic;
using System.Linq;

namespace NuGet.Messages
{
    public class CohortApprovedMessage : Message
    {
        public CohortApprovedMessage()
        {

        }

        public CohortApprovedMessage(string providerName, IEnumerable<string> apprenticeNames)
        {
            ProviderName = providerName;
            Apprentices = apprenticeNames.ToList();
        }
        //public void AddApprentice(string apprenticeName)
        //{
        //    Apprentices.Add(apprenticeName);
        //}

        public string ProviderName { get;  }

        public List<string> Apprentices { get; }=new List<string>();
    }
}
