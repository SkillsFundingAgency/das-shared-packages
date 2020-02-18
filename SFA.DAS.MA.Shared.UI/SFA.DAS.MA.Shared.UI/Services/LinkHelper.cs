using System;
using System.Linq;
using SFA.DAS.MA.Shared.UI.Models;

namespace SFA.DAS.MA.Shared.UI.Services
{
    public class LinkHelper : ILinkHelper
    {  
        private Models.ILinkCollection _linkCollection { get; }

        public LinkHelper(ILinkCollection linkCollection)
        {
            _linkCollection = linkCollection;
        }

        public string RenderListItemLink<T>(bool isSelected = false) where T : Link
        {
            return RenderLink<T>(() => "<li>", () => "</li>", isSelected);
        }

        public string RenderLink<T>(Func<string> before = null, Func<string> after = null, bool isSelected = false) where T : Link
        {
            if (_linkCollection.Links.OfType<T>().FirstOrDefault() != null)
            {
                var link = _linkCollection.Links.OfType<T>().First();
                link.IsSelected = isSelected;

                return $"{before?.Invoke()}{link.Render()}{after?.Invoke()}";
            }

            return string.Empty;
        }       
    }
}
