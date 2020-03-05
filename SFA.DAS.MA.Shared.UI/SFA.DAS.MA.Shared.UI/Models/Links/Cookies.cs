namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class Cookies : Link
    {
        public Cookies(string href, string @class = "") : base(href, @class: @class)
        {
        }

        public override string Render()
        {
            return $"<a href=\"{Href}\" class=\"{Class}\">Cookies</a>";
        }
    }
}
