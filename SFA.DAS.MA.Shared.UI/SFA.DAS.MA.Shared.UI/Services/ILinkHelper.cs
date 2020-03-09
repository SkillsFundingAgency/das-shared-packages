using System;
using SFA.DAS.MA.Shared.UI.Models;

namespace SFA.DAS.MA.Shared.UI.Services
{
    public interface ILinkHelper
    {
        string RenderListItemLink<T>(bool isSelected = false, string @class = "") where T : Models.Link;
        string RenderLink<T>(Func<string> before = null, Func<string> after = null, bool isSelected = false) where T : Link;
    }
}
