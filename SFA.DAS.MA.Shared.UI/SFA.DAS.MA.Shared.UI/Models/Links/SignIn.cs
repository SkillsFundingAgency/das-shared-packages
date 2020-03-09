namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class SignIn : Link
    {
        public SignIn(string href, string @class = "") : base(href, @class: @class)
        {
        }

        public override string Render()
        {
            return $"<a href = \"{Href}\" role=\"menuitem\" class=\"{Class}\">Sign in / Register</a>";
        }
    }
}
