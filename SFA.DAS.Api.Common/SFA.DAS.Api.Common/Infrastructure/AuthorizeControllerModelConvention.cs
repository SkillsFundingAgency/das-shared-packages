using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace SFA.DAS.Api.Common.Infrastructure
{
    public class AuthorizeControllerModelConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            controller.Filters.Add(PolicyNames.PolicyNameList.FirstOrDefault(c=>c.Equals(controller.ControllerName)) != null
                ? new AuthorizeFilter(controller.ControllerName)
                : new AuthorizeFilter(PolicyNames.Default));
        }
    }
}