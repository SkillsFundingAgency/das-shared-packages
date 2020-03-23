namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class ChangePassword : Link
    {
        public ChangePassword(string href, string @class = "") : base(href, @class: @class)
        {
        }

        public override string Render()
        {
            return $"<a href = \"{Href}\" class=\"{Class}\">Change your password</a>";
        }
    }
}
