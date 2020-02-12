namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class ChangePasssword : Link
    {
        public ChangePasssword(string href) : base(href)
        {
        }

        public override string Render()
        {
            return $"<a href = \"{Href}\" class=\"sub-menu-item\">Change your password</a>";
        }
    }
}
