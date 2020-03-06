namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class YourOrganisations : Link
    {
        private readonly string _class;
        private readonly string _role;

        public YourOrganisations(string href, string @class = "organisations", string role = "menuitem") : base(href, @class: @class)
        {
            _class = @class;
            _role = role;
        }

        public override string Render()
        {
            if (IsSelected)
            {
                return $"<a href = \"{Href}\" class=\"{_class} selected\" role=\"{_role}\">Your organisations and agreements</a>";
            }

            return $"<a href = \"{Href}\" class=\"{_class}\" role=\"{_role}\">Your organisations and agreements</a>";
        }
    }
}
