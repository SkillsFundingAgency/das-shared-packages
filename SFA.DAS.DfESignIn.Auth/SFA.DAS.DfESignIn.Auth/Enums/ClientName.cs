using System.ComponentModel;

namespace SFA.DAS.DfESignIn.Auth.Enums
{
    public enum ClientName
    {
        [Description("providers|pas")]
        ProviderRoatp = 1,
        [Description("support-tools")]
        BulkStop = 2,
        [Description("identify-data-locks")]
        DataLocks = 3,
        [Description("review")]
        Qa = 4,
        [Description("admin")]
        ServiceAdmin = 5,
        [Description("console")]
        SupportConsole = 6,
        [Description("providers|pas")]
        TraineeshipRoatp = 7
    }
}
