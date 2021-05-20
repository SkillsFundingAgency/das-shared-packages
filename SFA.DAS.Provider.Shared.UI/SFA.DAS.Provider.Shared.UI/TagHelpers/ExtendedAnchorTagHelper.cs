using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using SFA.DAS.Provider.Shared.UI.Extensions;
using SFA.DAS.Provider.Shared.UI.Models;

namespace SFA.DAS.Provider.Shared.UI.TagHelpers
{
    [HtmlTargetElement("a", Attributes = "asp-external-controller")]
    public class ExtendedAnchorTagHelper : AnchorTagHelper
    {
        private readonly IExternalUrlHelper _helper;

        [HtmlAttributeName("asp-external-action")]
        public string ExternalAction { get; set; }
        [HtmlAttributeName("asp-external-id")]
        public string ExternalId { get; set; }
        [HtmlAttributeName("asp-external-controller")]
        public string ExternalController { get; set; }
        [HtmlAttributeName("asp-external-subdomain")]
        public string ExternalSubDomain { get; set; }
        [HtmlAttributeName("asp-external-folder")]
        public string ExternalFolder { get; set; }
        [HtmlAttributeName("asp-external-querystring")]
        public string QueryString { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context,output);

            var urlParams = new UrlParameters
            {
                Id = ExternalId,
                Controller = ExternalController,
                Action = ExternalAction,
                SubDomain = ExternalSubDomain,
                Folder = ExternalFolder,
                QueryString = QueryString
            };
            output.Attributes.SetAttribute("href",_helper.GenerateUrl(urlParams));
        }

        public ExtendedAnchorTagHelper(IHtmlGenerator generator,IExternalUrlHelper helper) : base(generator)
        {
            _helper = helper;
        }
    }
}