namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class ChangeEmail : Link
    {
        public ChangeEmail(string href) : base(href)
        {
        }

        public override string Render()
        {
            return $"<a href = \"{Href}\" class=\"sub-menu-item\">Change your email address</a>";
        }
    }
}
