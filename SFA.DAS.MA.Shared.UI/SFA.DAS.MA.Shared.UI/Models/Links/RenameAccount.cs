namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class RenameAccount : Link
    {
        public RenameAccount(string href, string @class = "") : base(href, @class: @class)
        {
        }

        public override string Render()
        {
            return $"<a href = \"{Href}\" class=\"{Class}\">Rename account</a>";
        }
    }
}
