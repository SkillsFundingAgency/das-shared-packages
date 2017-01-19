namespace SFA.DAS.Events.Api.Types
{
    using System.ComponentModel;

    public enum PaymentStatus
    {
        [Description("Ready for approval")]
        PendingApproval = 0,

        Active = 1,

        Paused = 2,

        Withdrawn = 3,

        Completed = 4,

        Deleted = 5
    }
}
