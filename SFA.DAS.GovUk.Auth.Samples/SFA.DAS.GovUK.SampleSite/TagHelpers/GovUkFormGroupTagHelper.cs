using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SFA.DAS.GovUK.SampleSite.TagHelpers
{
    [HtmlTargetElement("govuk-form-group", Attributes = "asp-for")]
    public class GovUkFormGroupTagHelper : TagHelper
    {
        [HtmlAttributeName("asp-for")]
        public required ModelExpression For { get; set; }

        [ViewContext]
        [HtmlAttributeNotBound]
        public required ViewContext ViewContext { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var fieldName = For.Name;
            var modelState = ViewContext.ModelState;
            var hasError = modelState.TryGetValue(fieldName, out var entry) && entry.Errors.Any();
            var errorMessage = hasError ? entry?.Errors[0].ErrorMessage : null;

            output.TagName = "div";
            output.Attributes.SetAttribute("class", "govuk-form-group" + (hasError ? " govuk-form-group--error" : ""));

            var content = output.GetChildContentAsync().Result.GetContent();

            if (hasError)
            {
                var errorHtml = $@"
                    <span class=""govuk-error-message"">
                        <span class=""govuk-visually-hidden"">Error:</span> {errorMessage}
                    </span>";

                content = errorHtml + content;
            }

            output.Content.SetHtmlContent(content);
        }
    }
}