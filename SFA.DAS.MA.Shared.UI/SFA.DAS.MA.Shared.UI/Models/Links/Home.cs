namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class Home : Link
    {
        private readonly string _class;
        private readonly string _role;

        public Home(string href, string @class = "paye", string role = "menuitem") : base(href)
        {
            _class = @class;
            _role = role;
        }

        public override string Render()
        {
            if (IsSelected)
            {
                return $"<a href = \"{Href}\" class=\"{_class} selected\" role=\"menuitem\">Home</a>";
            }

            return $"<a href = \"{Href}\" class=\"{_class}\" role=\"menuitem\">Home</a>";
        }
    }
}
