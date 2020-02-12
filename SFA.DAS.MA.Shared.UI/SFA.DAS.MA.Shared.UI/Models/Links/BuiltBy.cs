namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class BuiltBy : Link
    {
        public BuiltBy(string href) : base(href)
        {        
        }

        public override string Render()
        {
            return $"<a href = \"{Href}\" target=\"_blank\">Education and Skills Funding Agency</a>";
        }
    }
}
