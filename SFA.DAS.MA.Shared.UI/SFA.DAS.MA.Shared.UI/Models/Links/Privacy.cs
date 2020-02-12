namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class Privacy : Link
    {
        public Privacy(string href) : base(href)
        {
        }

        public override string Render()
        {
            return $"<a href=\"{Href}\">Privacy</a>";
        }
    }
}
