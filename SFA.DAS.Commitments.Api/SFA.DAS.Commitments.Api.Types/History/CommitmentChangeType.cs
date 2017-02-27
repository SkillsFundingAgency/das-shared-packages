namespace SFA.DAS.Commitments.Api.Types.History
{
    public enum CommitmentChangeType
    {
        Create = 0,
        Delete = 1,
        CreateApprenticeship = 2,
        DeleteApprenticeship = 3,
        EditApprenticeship = 4,
        SendForReview = 5,
        SendForApproval = 6,
        FinalApproval = 7
    }
}