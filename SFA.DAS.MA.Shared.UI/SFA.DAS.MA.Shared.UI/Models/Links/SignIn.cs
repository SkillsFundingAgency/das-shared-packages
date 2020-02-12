namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class SignIn : Link
    {
        public SignIn(string href) : base(href)
        {
        }

        public override string Render()
        {
            return $"<a href = \"{Href}\" role=\"menuitem\">Sign in / Register</a>";
        }
    }
}
