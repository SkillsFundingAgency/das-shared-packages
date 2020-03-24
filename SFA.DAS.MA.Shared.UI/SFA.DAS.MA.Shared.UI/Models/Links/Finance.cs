namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class Finance : Link
    {
        private readonly string _class;
        private readonly bool _isLegacy;
        private readonly string _selectedClass;

        public Finance(string href, string @class = "finance", bool isLegacy = false) : base(href, @class: @class)
        {
            _class = @class;
            _isLegacy = isLegacy;
            _selectedClass = _isLegacy ? "selected" : "das-navigation__link--current";
        }

        public override string Render()
        {
            if(IsSelected)
            {
                return $"<a href = \"{Href}\" class=\"{_class} {_selectedClass}\">Finance</a>";
            }

            return $"<a href = \"{Href}\" class=\"{_class}\">Finance</a>";
        }
    }
}
