namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class CrownCopyright : Link
    {
        public CrownCopyright(string href) : base(href)
        {
        }

        public override string Render()
        {
            return $"<a href = \"{Href}\">© Crown copyright</a>";
        }
    }
}
