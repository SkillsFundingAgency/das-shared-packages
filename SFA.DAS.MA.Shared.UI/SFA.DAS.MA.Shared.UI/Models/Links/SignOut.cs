namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class SignOut : Link
    {
        public SignOut(string href) : base(href)
        {
        }

        public override string Render()
        {
            return $"<a href = \"{Href}\" role=\"menuitem\">Sign out</a>";
        }
    }
}
