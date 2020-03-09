using SFA.DAS.MA.Shared.UI.Services;

namespace SFA.DAS.MA.Shared.UI.Models
{
    public interface IFooterViewModel : ILinkCollection, ILinkHelper
    {
        bool UseLegacyStyles { get; }
    }
}
