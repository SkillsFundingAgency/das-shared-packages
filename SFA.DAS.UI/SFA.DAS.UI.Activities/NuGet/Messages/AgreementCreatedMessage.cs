using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuGet.Messages
{
    public class AgreementCreatedMessage : Message
    {
        public AgreementCreatedMessage()
        {

        }

        public AgreementCreatedMessage(long accountId, long legalEntityId, long aggreementId, string companyName)
        {
            AccountId = accountId;
            LegalEntityId = legalEntityId;
            AgreementId = aggreementId;
            CompanyName = companyName;
        }

        public string CompanyName { get; }

        public long AccountId { get; }
        public long LegalEntityId { get; }
        public long AgreementId { get; }
    }
}
