namespace SFA.DAS.Commitments.Api.Types
{
    public sealed class CommitmentRequest
    {
        public string UserId { get; set; }

        public Commitment Commitment { get; set; }
    }
}
