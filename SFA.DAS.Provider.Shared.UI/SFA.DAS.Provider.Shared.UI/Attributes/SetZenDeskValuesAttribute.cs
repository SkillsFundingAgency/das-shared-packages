using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace SFA.DAS.Provider.Shared.UI.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class SetZenDeskValuesAttribute : ResultFilterAttribute
    {
        public string SnippetKey { get; }
        public string SectionId { get; }

        public SetZenDeskValuesAttribute(string snippetKey, string sectionId)
        {
            SnippetKey = snippetKey;
            SectionId = sectionId;
        }

        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (!(context.Controller is Controller controller)) return;
            controller.ViewData[ViewDataKeys.ZenDeskSnippetKey] = SnippetKey;
            controller.ViewData[ViewDataKeys.ZenDeskSectionId] = SectionId;
        }
    }
}