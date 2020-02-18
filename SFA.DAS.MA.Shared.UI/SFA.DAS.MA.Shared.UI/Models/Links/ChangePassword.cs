namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class ChangePassword : Link
    {
        public ChangePassword(string href) : base(href)
        {
        }

        public override string Render()
        {
            return $"<a href = \"{Href}\" class=\"sub-menu-item\">Change your password</a>";
        }
    }
}
