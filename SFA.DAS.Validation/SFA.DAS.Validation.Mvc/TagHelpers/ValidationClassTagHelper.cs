using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace SFA.DAS.Validation.Mvc.TagHelpers
{
    [HtmlTargetElement(Attributes = ForAttributeName + "," + ValidationClassAttributeName)] 
    public class ValidationClassTagHelper : TagHelper
    {
        [HtmlAttributeName(ForAttributeName)]
        public ModelExpression For { get; set; }
        
        [HtmlAttributeName(ValidationClassAttributeName)]
        public string ValidationClass { get; set; }
        
        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }
    
        private const string ForAttributeName = "asp-for";
        private const string ValidationClassAttributeName = "validation-class";
        
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            if (ViewContext.ModelState.TryGetValue(For.Name, out var modelStateEntry) && modelStateEntry.Errors.Count > 0)
            {
                output.AddClass(ValidationClass, HtmlEncoder.Default);
            }
        }
    }
}