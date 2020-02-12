namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class YourTeam : Link
    {
        private readonly string _class;
        private readonly string _role;

        public YourTeam(string href, string @class = "paye", string role = "menuitem") : base(href)
        {
            _class = @class;
            _role = role;
        }

        public override string Render()
        {
            if (IsSelected)
            {
                return $"<a href = \"{Href}\" class=\"{_class} selected\" role=\"menuitem\">Your team</a>";
            }

            return $"<a href = \"{Href}\" class=\"{_class}\" role=\"menuitem\">Your team</a>";
        }
    }
}
