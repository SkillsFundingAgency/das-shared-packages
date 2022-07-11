namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class TermsOfUse : Link
    {
        public TermsOfUse(string href, string @class = "govuk-footer__link") : base(href, @class: @class)
        {
        }

        public override string Render()
        {
            return $"<a href=\"{Href}\" target=\"_blank\" class=\"{Class}\">Terms of use</a>";
        }
    }
}
