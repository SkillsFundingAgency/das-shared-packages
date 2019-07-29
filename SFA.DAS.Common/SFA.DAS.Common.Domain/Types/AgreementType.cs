using System.ComponentModel;

namespace SFA.DAS.Common.Domain.Types
{
    public enum AgreementType : byte
    {
        [Description("Levy")] Levy = 0,
        [Description("Expression of Interest")] NoneLevyExpressionOfInterest = 1,
    }
}
