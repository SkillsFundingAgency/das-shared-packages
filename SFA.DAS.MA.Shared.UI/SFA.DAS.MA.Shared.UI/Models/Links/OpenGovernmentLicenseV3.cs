namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class OpenGovernmentLicenseV3 : Link
    {
        public OpenGovernmentLicenseV3(string href, string @class = "") : base(href, @class: @class)
        {
        }

        public override string Render()
        {
            return $"<a href = \"{Href}\" rel=\"license\" class=\"{Class}\">Open Government Licence v3.0</a>";
        }
    }
}
