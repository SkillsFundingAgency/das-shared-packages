namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class Feedback : Link
    {
        public Feedback(string href) : base(href)
        {
        }

        public override string Render()
        {
            return $"<a href = \"{Href}\" target=\"_blank\">Feedback</a>";
        }
    }
}
