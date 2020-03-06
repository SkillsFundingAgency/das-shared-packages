using System;
using System.Collections.Generic;
using System.Text;

namespace SFA.DAS.MA.Shared.UI.Models.Links
{
    public class ManageApprenticeships : Link
    {
        public ManageApprenticeships(string href, string @class = "") : base(href, @class: @class)
        {
        }

        public override string Render()
        {
            return $"<a href = \"{Href}\" id=\"proposition-name\" class=\"{Class}\">Manage apprenticeships</a>";
        }
    }
}
