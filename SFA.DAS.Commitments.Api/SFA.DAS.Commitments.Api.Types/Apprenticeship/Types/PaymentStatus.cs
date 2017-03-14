using System.ComponentModel;

namespace SFA.DAS.Commitments.Api.Types.Apprenticeship.Types
{
    public enum PaymentStatus : short
    {
        [Description("Ready for approval")] PendingApproval = 0,
        Active = 1,
        Paused = 2,
        Withdrawn = 3,
        Completed = 4,
        Deleted = 5
    }
}
