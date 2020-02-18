namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class NotificationSettings : Link
    {
        public NotificationSettings(string href) : base(href)
        {
        }

        public override string Render()
        {
            return $"<a href = \"{Href}\" class=\"sub-menu-item\">Notification settings</a>";
        }
    }
}
