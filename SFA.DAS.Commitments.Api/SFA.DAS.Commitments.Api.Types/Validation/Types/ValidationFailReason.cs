namespace SFA.DAS.Commitments.Api.Types.Validation.Types
{
    public enum ValidationFailReason
    {
        None = 0,
        OverlappingStartDate = 1,
        OverlappingEndDate = 2,
        DateEmbrace = 3,
        DateWithin = 4
    }
}