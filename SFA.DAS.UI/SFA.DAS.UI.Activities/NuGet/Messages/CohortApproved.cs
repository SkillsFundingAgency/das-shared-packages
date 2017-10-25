

using System.Collections.Generic;

namespace NuGet.Messages
{
    public class CohortApproved : Message
    {

        public void AddApprentice(string apprenticeName)
        {
            Apprentices.Add(apprenticeName);
        }

        public string ProviderName { get; set; }

        public List<string> Apprentices { get; }=new List<string>();
    }
}
