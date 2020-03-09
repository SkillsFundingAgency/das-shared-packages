namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class PayeSchemes : Link
    {
        private readonly string _class;
        private readonly string _role;

        public PayeSchemes(string href, string @class = "paye", string role = "menuitem") : base(href, @class: @class)
        {
            _class = @class;
            _role = role;
        }

        public override string Render()
        {
            if (IsSelected)
            {
                return $"<a href = \"{Href}\" class=\"{_class} selected\" role=\"{_role}\">PAYE schemes</a>";
            }

            return $"<a href = \"{Href}\" class=\"{_class}\" role=\"{_role}\">PAYE schemes</a>";
        }
    }
}
