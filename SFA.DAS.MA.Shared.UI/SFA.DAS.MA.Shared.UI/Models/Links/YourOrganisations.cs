namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class YourOrganisations : Link
    {
        private readonly string _class;
        private readonly bool _isLegacy;
        private readonly string _selectedClass;

        public YourOrganisations(string href, string @class = "organisations", bool isLegacy = false) : base(href, @class: @class)
        {
            _class = @class;
            _isLegacy = isLegacy;
            _selectedClass = _isLegacy ? "selected" : "das-navigation__link--current";
        }

        public override string Render()
        {
            if (IsSelected)
            {
                return $"<a href = \"{Href}\" class=\"{_class} {_selectedClass}\">Your organisations and agreements</a>";
            }

            return $"<a href = \"{Href}\" class=\"{_class}\">Your organisations and agreements</a>";
        }
    }
}
