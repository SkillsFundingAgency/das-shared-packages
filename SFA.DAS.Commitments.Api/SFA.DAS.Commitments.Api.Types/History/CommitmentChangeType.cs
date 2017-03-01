namespace SFA.DAS.Commitments.Api.Types.History
{
    public enum CommitmentChangeType
    {
        Created = 0,
        Deleted = 1,
        CreatedApprenticeship = 2,
        DeletedApprenticeship = 3,
        EditedApprenticeship = 4,
        SentForReview = 5,
        SentForApproval = 6,
        FinalApproval = 7
    }
}