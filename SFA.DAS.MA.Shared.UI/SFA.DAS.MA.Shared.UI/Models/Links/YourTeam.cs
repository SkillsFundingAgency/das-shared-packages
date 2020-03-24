namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class YourTeam : Link
    {
        private readonly string _class;
        private readonly bool _isLegacy;
        private readonly string _selectedClass;

        public YourTeam(string href, string @class = "team", bool isLegacy = false) : base(href, @class: @class)
        {
            _class = @class;
            _isLegacy = isLegacy;
            _selectedClass = _isLegacy ? "selected" : "das-navigation__link--current";
        }

        public override string Render()
        {
            if (IsSelected)
            {
                return $"<a href = \"{Href}\" class=\"{_class} {_selectedClass}\">Your team</a>";
            }

            return $"<a href = \"{Href}\" class=\"{_class}\">Your team</a>";
        }
    }
}
