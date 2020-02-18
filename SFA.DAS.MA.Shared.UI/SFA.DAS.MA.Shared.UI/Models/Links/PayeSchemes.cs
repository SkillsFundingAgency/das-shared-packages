namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class PayeSchemes : Link
    {
        private readonly string _class;
        private readonly string _role;

        public PayeSchemes(string href, string @class = "paye", string role = "menuitem") : base(href)
        {
            _class = @class;
            _role = role;
        }

        public override string Render()
        {
            if (IsSelected)
            {
                return $"<a href = \"{Href}\" class=\"{_class} selected\" role=\"menuitem\">PAYE schemes</a>";
            }

            return $"<a href = \"{Href}\" class=\"{_class}\" role=\"menuitem\">PAYE schemes</a>";
        }
    }
}
