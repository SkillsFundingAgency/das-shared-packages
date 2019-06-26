using System.ComponentModel;

namespace SFA.DAS.Common.Domain.Types
{
    public enum ApprenticeshipEmployerType : byte
    {
        [Description("Non Levy")] NonLevy = 0,
        [Description("Levy")] Levy = 1,
    }
}
