namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class ChangeEmail : Link
    {
        public ChangeEmail(string href, string @class = "") : base(href, @class: @class)
        {
        }

        public override string Render()
        {
            return $"<a href = \"{Href}\" class=\"sub-menu-item\" class=\"{Class}\">Change your email address</a>";
        }
    }
}
