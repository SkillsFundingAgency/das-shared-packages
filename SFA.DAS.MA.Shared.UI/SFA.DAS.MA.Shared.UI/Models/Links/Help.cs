namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class Help : Link
    {
        public Help(string href, string @class = "govuk-footer__link") : base(href, @class: @class)
        {
        }

        public override string Render()
        {
            return $"<a href=\"{Href}\" target=\"_blank\" class=\"{Class}\">Help</a>";
        }
    }
}
