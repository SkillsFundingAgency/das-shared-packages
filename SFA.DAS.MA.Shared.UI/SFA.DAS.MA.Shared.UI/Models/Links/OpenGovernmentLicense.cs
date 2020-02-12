namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class OpenGovernmentLicense : Link
    {
        public OpenGovernmentLicense(string href) : base(href)
        {
        }

        public override string Render()
        {
            return $"<a href = \"{Href}\" rel=\"license\">Open Government Licence</a>";
        }
    }
}
