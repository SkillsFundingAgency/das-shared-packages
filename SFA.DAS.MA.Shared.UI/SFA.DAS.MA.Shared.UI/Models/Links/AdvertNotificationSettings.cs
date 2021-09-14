namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class AdvertNotificationSettings : Link
    {
        public AdvertNotificationSettings(string href, string @class = "") : base(href, @class: @class)
        {
        }

        public override string Render()
        {
            return $"<a href = \"{Href}\" class=\"{Class}\">Manage your advert notifications</a>";
        }
    }
}
