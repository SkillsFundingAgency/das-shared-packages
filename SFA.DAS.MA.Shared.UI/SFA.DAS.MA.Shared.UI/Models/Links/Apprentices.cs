namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class Apprentices : Link
    {
        private readonly string _class;
        private readonly string _role;

        public Apprentices(string href, string @class = "EmployerCommitments", string role = "menuitem") : base(href)
        {
            _class = @class;
            _role = role;
        }

        public override string Render()
        {
            if (IsSelected)
            {
                return $"<a href = \"{Href}\" class=\"{_class} selected\" role=\"menuitem\">Apprentices</a>";
            }

            return $"<a href = \"{Href}\" class=\"{_class}\" role=\"menuitem\">Apprentices</a>";
        }
    }
}
