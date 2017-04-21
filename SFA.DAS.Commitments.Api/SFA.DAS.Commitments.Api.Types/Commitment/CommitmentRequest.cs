namespace SFA.DAS.Commitments.Api.Types.Commitment
{
    public sealed class CommitmentRequest
    {
        public string UserId { get; set; }

        public Commitment Commitment { get; set; }

        public string Message { get; set; }
    }
}
