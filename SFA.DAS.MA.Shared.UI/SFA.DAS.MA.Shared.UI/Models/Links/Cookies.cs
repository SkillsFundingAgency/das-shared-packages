namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class Cookies : Link
    {
        public Cookies(string href) : base(href)
        {
        }

        public override string Render()
        {
            return $"<a href=\"{Href}\">Cookies</a>";
        }
    }
}
