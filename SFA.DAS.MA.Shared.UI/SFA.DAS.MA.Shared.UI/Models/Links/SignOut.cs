namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class SignOut : Link
    {
        public SignOut(string href, string @class = "") : base(href, @class: @class)
        {
        }

        public override string Render()
        {
            return $"<a href = \"{Href}\" role=\"menuitem\" class=\"{Class}\" esfa-automation=\"sign-out\">Sign out</a>";
        }
    }
}
