namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class GovUk : Link
    {
        private readonly string _class;

        public GovUk(string href, string @class = "content") : base(href)
        {
            _class = @class;
        }

        public override string Render()
        {
            return $"<a href = \"{Href}\" class=\"{_class}\"  title=\"Go to the GOV.UK homepage\" id=\"logo\"><img src=\"https://assets.publishing.service.gov.uk/static/gov.uk_logotype_crown_invert_trans-203e1db49d3eff430d7dc450ce723c1002542fe1d2bce661b6d8571f14c1043c.png\" alt=\"\" width=\"36\" height=\"32\">GOV.UK</a>";
        }
    }
}
