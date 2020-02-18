namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class Help : Link
    {
        public Help(string href) : base(href)
        {
        }

        public override string Render()
        {
            return $"<a href=\"{Href}\" target=\"_blank\">Help</a>";
        }
    }
}
