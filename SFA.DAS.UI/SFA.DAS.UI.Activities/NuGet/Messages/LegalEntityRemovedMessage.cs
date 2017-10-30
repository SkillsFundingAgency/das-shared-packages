using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.Messages
{
    public class LegalEntityRemovedMessage : Message
    {
        public LegalEntityRemovedMessage()
        {

        }

        public LegalEntityRemovedMessage(long accountId, long legalEntityId, long aggreementId,bool agreementSigned)
        {
            AccountId = accountId;
            LegalEntityId = legalEntityId;
            AgreementId = aggreementId;
            AgreementSigned = agreementSigned;
        }

        public string CompanyName { get; set; }
        public long AccountId { get; }
        public long LegalEntityId { get; }
        public long AgreementId { get;  }
        public bool AgreementSigned { get;  }
    }
}
