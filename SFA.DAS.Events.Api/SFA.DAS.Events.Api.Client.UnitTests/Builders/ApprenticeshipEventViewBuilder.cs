using System;
using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.Events.Api.Client.UnitTests.Builders
{
    public class ApprenticeshipEventViewBuilder
    {
        private DateTime _createdOn = DateTime.Now.AddDays(-1);
        private string _employerAccountId = "ABC123";
        private string _event = "Test";
        private int _id = 87435;
        private string _providerId = "ZZZ999";
        private AgreementStatus _agreementStatus = AgreementStatus.BothAgreed;
        private int _apprenticeshipId = 123;
        private string _learnerId = "LID";
        private int _paymentOrder = 1;
        private PaymentStatus _paymentStatus = PaymentStatus.Completed;
        private DateTime _trainingEndDate = DateTime.Now.AddYears(3);
        private string _trainingId = "ABC123";
        private DateTime _trainingStartDate = DateTime.Now.AddYears(-1);
        private decimal _trainingTotalCost = 10000.34m;
        private TrainingTypes _trainingTypes = TrainingTypes.Standard;
        private string _legalEntityId = "LE ID";
        private string _legalEntityName = "LE Name";
        private string _legalEntityOrganisationType = "LE Org Type";

        public ApprenticeshipEventView Build()
        {
            return new ApprenticeshipEventView
            {
                CreatedOn = _createdOn,
                EmployerAccountId = _employerAccountId,
                Event = _event,
                Id = _id,
                ProviderId = _providerId,
                AgreementStatus = _agreementStatus,
                ApprenticeshipId = _apprenticeshipId,
                LearnerId = _learnerId,
                PaymentOrder = _paymentOrder,
                PaymentStatus = _paymentStatus,
                TrainingEndDate = _trainingEndDate,
                TrainingId = _trainingId,
                TrainingStartDate = _trainingStartDate,
                TrainingTotalCost = _trainingTotalCost,
                TrainingType = _trainingTypes,
                LegalEntityId = _legalEntityId,
                LegalEntityName = _legalEntityName,
                LegalEntityOrganisationType = _legalEntityOrganisationType
            };
        }
    }
}
