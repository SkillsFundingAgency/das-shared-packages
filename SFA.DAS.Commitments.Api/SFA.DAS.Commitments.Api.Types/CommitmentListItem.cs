namespace SFA.DAS.Commitments.Api.Types
{
    public class CommitmentListItem
    {
        public long Id { get; set; }
        public string Reference { get; set; }
        public long EmployerAccountId { get; set; }
        public string EmployerAccountName { get; set; }
        public string LegalEntityId { get; set; }
        public string LegalEntityName { get; set; }
        public long? ProviderId { get; set; }
        public string ProviderName { get; set; }
        public CommitmentStatus CommitmentStatus { get; set; }
    }
}