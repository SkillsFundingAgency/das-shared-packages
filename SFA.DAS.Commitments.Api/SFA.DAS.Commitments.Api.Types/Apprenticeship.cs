using System;

namespace SFA.DAS.Commitments.Api.Types
{
    public class Apprenticeship
    {
        public long Id { get; set; }
        public long CommitmentId { get; set; }
        public string ApprenticeName { get; set; }
        public string ULN { get; set; }
        public string TrainingId { get; set; } //standard or framework
        public decimal? Cost { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ApprenticeshipStatus Status { get; set; }
        public AgreementStatus AgreementStatus { get; set; }
    }
}