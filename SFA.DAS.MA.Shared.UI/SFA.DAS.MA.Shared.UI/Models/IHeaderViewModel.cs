using SFA.DAS.MA.Shared.UI.Configuration;
using SFA.DAS.MA.Shared.UI.Services;

namespace SFA.DAS.MA.Shared.UI.Models
{
    public interface IHeaderViewModel : ILinkCollection, ILinkHelper
    {
        bool MenuIsHidden { get; }
        string SelectedMenu { get;}
        IUserContext UserContext { get; }
        void HideMenu();
        void SelectMenu(string menu);
    }
}
