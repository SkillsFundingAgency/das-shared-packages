
namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class Recruitment : Link
    {
        private readonly string _class;
        private readonly string _role;

        public Recruitment(string href, string @class = "", string role = "menuitem") : base(href, @class: @class)
        {
            _class = @class;
            _role = role;
        }

        public override string Render()
        {
            if (IsSelected)
            {
                return $"<a href = \"{Href}\" class=\"{_class} selected\" role=\"{_role}\">Recruitment</a>";
            }

            return $"<a href = \"{Href}\" class=\"{_class}\" role=\"{_role}\">Recruitment</a>";
        }
    }
}
