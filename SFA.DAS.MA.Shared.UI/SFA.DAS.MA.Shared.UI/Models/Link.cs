namespace SFA.DAS.MA.Shared.UI.Models
{
    public abstract class Link
    {
        public string Class { get; set; }
        public string Href { get; set; }
        public bool IsSelected { get; set; }

        public Link(string href, bool isSelected = false, string @class = "")
        {
            Href = href;
            IsSelected = isSelected;
            Class = @class;
        }

        public abstract string Render();
    }
}
