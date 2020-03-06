namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class Feedback : Link
    {
        public Feedback(string href, string @class = "") : base(href, @class: @class)
        {
        }

        public override string Render()
        {
            return $"<a href = \"{Href}\" target=\"_blank\" class=\"{Class}\">Feedback</a>";
        }
    }
}
