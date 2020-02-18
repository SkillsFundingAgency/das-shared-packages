using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class ManageApprenticeships : Link
    {
        public ManageApprenticeships(string href) : base(href)
        {
        }

        public override string Render()
        {
            return $"<a href = \"{Href}\" id=\"proposition-name\">Manage apprenticeships</a>";
        }
    }
}
