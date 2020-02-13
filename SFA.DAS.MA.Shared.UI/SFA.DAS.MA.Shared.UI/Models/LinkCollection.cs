using System.Collections.Generic;
using System.Linq;

namespace SFA.DAS.MA.Shared.UI.Models
{
    public class LinkCollection : ILinkCollection
    {
        public IReadOnlyList<Link> Links
        {
            get
            {
                return _links.AsReadOnly();
            }
        }

        private List<Link> _links { get; }

        public LinkCollection()
        {
            _links = new List<Link>();
        }

        public void AddOrUpdateLink<T>(T link) where T : Link
        {
            if (_links.OfType<T>().FirstOrDefault() != null)
            {
                RemoveLink<T>();
                AddOrUpdateLink(link);
            }
            else
            {
                _links.Add(link);
            }
        }

        public void RemoveLink<T>() where T : Link
        {
            var link = _links.OfType<T>().FirstOrDefault();
            if (link != null)
            {
                _links.Remove(link);
            }
        }
    }
}
