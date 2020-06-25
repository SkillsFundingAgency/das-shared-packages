using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.Provider.Shared.UI.Models
{
    public class GaData
    {
        public string DataLoaded { get; set; } = "dataLoaded";
        public IDictionary<string, string> Extras { get; set; } = new Dictionary<string, string>();
        public string UkPrn { get; set; }
        public string Vpv { get; set; }
        public string UserId { get; set; }
        public string Org { get; set; }
    }
}
