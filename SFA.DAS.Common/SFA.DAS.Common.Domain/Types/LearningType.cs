using System.ComponentModel;

namespace SFA.DAS.Common.Domain.Types
{
    public enum LearningType : byte
    {
        [Description("Apprenticeship")] Apprenticeship = 0,
        [Description("Foundation Apprenticeship")] FoundationApprenticeship = 1,
        [Description("ApprenticeshipUnit")] ApprenticeshipUnit = 2
    }
}
