namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class YourAccounts : Link
    {
        public YourAccounts(string href, string @class = "") : base(href, @class: @class)
        {
        }

        public override string Render()
        {
            return $"<a href = \"{Href}\" role=\"menuitem\" class=\"sub-menu-item\">Your accounts</a>";
        }
    }
}
