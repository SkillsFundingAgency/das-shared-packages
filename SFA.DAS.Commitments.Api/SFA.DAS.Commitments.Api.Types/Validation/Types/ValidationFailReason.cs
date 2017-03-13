namespace SFA.DAS.Commitments.Api.Types.Validation
{
    public enum ValidationFailReason
    {
        OverlappingStartDate = 0,
        OverlappingEndDate = 1,
        DateEmbrace = 2,
        DateWithin = 3
    }
}