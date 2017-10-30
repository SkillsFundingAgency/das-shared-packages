using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.Messages
{
    public class AgreementSignedMessage : Message
    {
        public AgreementSignedMessage()
        {

        }

        public AgreementSignedMessage(long accountId, long legalEntityId, long aggreementId, string providerName)
        {
            AccountId = accountId;
            LegalEntityId = legalEntityId;
            AgreementId = aggreementId;
            ProviderName = providerName;
        }

        public string ProviderName { get; set; }

        public long AccountId { get;}
        public long LegalEntityId { get;  }
        public long AgreementId { get;  }
    }
}
