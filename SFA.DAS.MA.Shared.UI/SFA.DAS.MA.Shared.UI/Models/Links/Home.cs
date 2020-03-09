namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class Home : Link
    {
        private readonly string _class;
        private readonly string _role;

        public Home(string href, string @class = "paye", string role = "menuitem") : base(href, @class: @class)
        {
            _class = @class;
            _role = role;
        }

        public override string Render()
        {
            if (IsSelected)
            {
                return $"<a href = \"{Href}\" class=\"{_class} selected\" role=\"{_role}\">Home</a>";
            }

            return $"<a href = \"{Href}\" class=\"{_class}\" role=\"{_role}\">Home</a>";
        }
    }
}
