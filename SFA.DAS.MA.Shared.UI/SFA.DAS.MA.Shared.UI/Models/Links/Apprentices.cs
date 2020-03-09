namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class Apprentices : Link
    {
        private readonly string _role;

        public Apprentices(string href, string @class = "EmployerCommitments", string role = "menuitem") : base(href, @class: @class)
        {
            _role = role;
        }

        public override string Render()
        {
            if (IsSelected)
            {
                return $"<a href = \"{Href}\" class=\"{Class} selected\" role=\"{_role}\">Apprentices</a>";
            }

            return $"<a href = \"{Href}\" class=\"{Class}\" role=\"{_role}\">Apprentices</a>";
        }
    }
}
