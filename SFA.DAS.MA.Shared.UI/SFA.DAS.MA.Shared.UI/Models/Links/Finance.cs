namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class Finance : Link
    {
        private readonly string _class;
        private readonly string _role;

        public Finance(string href, string @class = "finance", string role = "menuitem") : base(href)
        {
            _class = @class;
            _role = role;
        }

        public override string Render()
        {
            if(IsSelected)
            {
                return $"<a href = \"{Href}\" class=\"{_class} selected\" role=\"menuitem\">Finance</a>";
            }
            return $"<a href = \"{Href}\" class=\"{_class}\" role=\"menuitem\">Finance</a>";
        }
    }
}
