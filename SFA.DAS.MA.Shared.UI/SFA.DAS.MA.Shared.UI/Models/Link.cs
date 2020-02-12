namespace SFA.DAS.MA.Shared.UI.Models
{
    public abstract class Link
    {
        public string Href { get; set; }
        public bool IsSelected { get; set; }

        public Link(string href, bool isSelected = false)
        {
            Href = href;
            IsSelected = isSelected;
        }

        public abstract string Render();
    }
}
