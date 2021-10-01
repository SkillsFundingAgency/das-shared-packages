using SFA.DAS.MA.Shared.UI.Configuration;
using SFA.DAS.MA.Shared.UI.Services;

namespace SFA.DAS.MA.Shared.UI.Models
{
    public interface IHeaderViewModel : ILinkCollection, ILinkHelper
    {
        bool MenuIsPartial { get; }
        bool MenuIsHidden { get; }
        bool NavBarIsHidden { get; }
        string SelectedMenu { get;}
        IUserContext UserContext { get; }
        void HideMenu(bool partial = true);
        void SelectMenu(string menu);
        bool UseLegacyStyles { get; }
    }
}
