namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class TermsAndConditions : Link
    {
        public TermsAndConditions(string href, string @class = "govuk-footer__link") : base(href, @class: @class)
        {
        }

        public override string Render()
        {
            return $"<a href=\"{Href}\" target=\"_blank\" class=\"{Class}\">Terms and conditions</a>";
        }
    }
}
