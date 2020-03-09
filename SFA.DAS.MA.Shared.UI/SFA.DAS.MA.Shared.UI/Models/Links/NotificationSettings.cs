namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class NotificationSettings : Link
    {
        public NotificationSettings(string href, string @class = "") : base(href, @class: @class)
        {
        }

        public override string Render()
        {
            return $"<a href = \"{Href}\" class=\"sub-menu-item\" class=\"{Class}\">Notification settings</a>";
        }
    }
}
