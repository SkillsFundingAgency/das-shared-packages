using System.ComponentModel;

namespace SFA.DAS.Commitments.Api.Types
{
    public enum ApprenticeshipStatus : short
    {
        [Description("Ready for approval")]
        ReadyForApproval = 0,
        Approved = 1,
        Paused = 2
    }
}