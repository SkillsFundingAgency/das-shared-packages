using System;

using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;

namespace SFA.DAS.Commitments.Api.Types.Apprenticeship
{
    public class Apprenticeship
    {
        public long Id { get; set; }
        public long CommitmentId { get; set; }
        public long EmployerAccountId { get; set; }
        public long ProviderId { get; set; }
        public string Reference { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string NINumber { get; set; }
        public string ULN { get; set; }
        public TrainingType TrainingType { get; set; }
        public string TrainingCode { get; set; }
        public string TrainingName { get; set; }
        public decimal? Cost { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public AgreementStatus AgreementStatus { get; set; }
        public string EmployerRef { get; set; }
        public string ProviderRef { get; set; }
        public bool CanBeApproved { get; set; }
        public Originator? PendingUpdateOriginator { get; set; }
        public string ProviderName { get; set; }
        public string LegalEntityName { get; set; }

        public string ApprenticeshipName => $"{FirstName} {LastName}";
    }
}
