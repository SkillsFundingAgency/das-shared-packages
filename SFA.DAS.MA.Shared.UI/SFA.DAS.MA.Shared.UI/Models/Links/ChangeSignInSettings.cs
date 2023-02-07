namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class ChangeSignInSettings : Link
    {
        public ChangeSignInSettings(string href, string @class = "") : base(href, @class: @class)
        {
        }

        public override string Render()
        {
            return $"<a href = \"{Href}\" class=\"{Class}\">Change your sign-in details</a>";
        }
    }
}
