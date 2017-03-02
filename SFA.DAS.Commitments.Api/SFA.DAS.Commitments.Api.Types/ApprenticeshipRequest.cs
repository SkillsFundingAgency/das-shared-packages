namespace SFA.DAS.Commitments.Api.Types
{
    public sealed class ApprenticeshipRequest
    {
        public Apprenticeship Apprenticeship { get; set; }

        public string UserId { get; set; }
    }

    public class DeleteRequest
    {
        public string UserId { get; set; }
    }
}
