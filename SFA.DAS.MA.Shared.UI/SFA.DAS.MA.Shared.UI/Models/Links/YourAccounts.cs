namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class YourAccounts : Link
    {
        public YourAccounts(string href) : base(href)
        {
        }

        public override string Render()
        {
            return $"<a href = \"{Href}\" role=\"menuitem\" class=\"sub-menu-item\">Your accounts</a>";
        }
    }
}
