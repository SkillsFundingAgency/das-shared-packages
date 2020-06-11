namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class Recruitment : Link
    {
        private readonly string _class;
        private readonly bool _isLegacy;
        private readonly string _selectedClass;

        public Recruitment(string href, string @class = "recruitment", bool isLegacy = false) : base(href, @class: @class)
        {
            _class = @class;
            _isLegacy = isLegacy;
            _selectedClass = _isLegacy ? "selected" : "das-navigation__link--current";
        }

        public override string Render()
        {
            if (IsSelected)
            {
                return $"<a href = \"{Href}\" class=\"{_class} {_selectedClass}\">Adverts</a>";
            }

            return $"<a href = \"{Href}\" class=\"{_class}\">Adverts</a>";
        }
    }
}
