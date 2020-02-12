namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class OpenGovernmentLicenseV3 : Link
    {
        public OpenGovernmentLicenseV3(string href) : base(href)
        {
        }

        public override string Render()
        {
            return $"<a href = \"{Href}\" rel=\"license\">Open Government Licence v3.0</a>";
        }
    }
}
