using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SFA.DAS.Provider.Shared.UI.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class HideNavigationBarAttribute : ResultFilterAttribute
    {
        private bool HideAccountHeader {get;}
        private bool HideNavigationLinks {get;}

        public HideNavigationBarAttribute()
        {
            HideAccountHeader = true;
            HideNavigationLinks = true;
        }
        
        public HideNavigationBarAttribute(bool hideAccountHeader, bool hideNavigationLinks)
        {
            HideAccountHeader = hideAccountHeader;
            HideNavigationLinks = hideNavigationLinks;
        }
        
        public override void OnResultExecuting(ResultExecutingContext context)
        {
            if (!(context.Controller is Controller controller)) return;
            
            if (HideAccountHeader)
            {
                controller.ViewData[ViewDataKeys.HideAccountHeader] = true;
            }

            if(HideNavigationLinks)
            {
                controller.ViewData[ViewDataKeys.HideNavigationLinks] = true;
            }
        }
    }
}
