namespace SFA.DAS.Commitments.Api.Types.Apprenticeship.Types
{
    public enum RecordStatus
    {
        NoActionNeeded = 0,
        ChangesPending = 1,
        ChangesForReview = 2,
        ChangeRequested = 3,
        IlrDataMismatch = 4,
        IlrChangesPending = 5
    }
}