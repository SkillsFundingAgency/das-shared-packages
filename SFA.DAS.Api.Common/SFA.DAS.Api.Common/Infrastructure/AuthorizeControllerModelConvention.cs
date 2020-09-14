using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Authorization;

namespace SFA.DAS.Api.Common.Infrastructure
{
    public class AuthorizeControllerModelConvention : IControllerModelConvention
    {
        private readonly IEnumerable<string> _policyNames;

        public AuthorizeControllerModelConvention(IEnumerable<string> policyNames)
        {
            _policyNames = policyNames;
        }

        public void Apply(ControllerModel controller)
        {
            controller.Filters.Add(_policyNames.FirstOrDefault(c=>c.Equals(controller.ControllerName)) != null
                ? new AuthorizeFilter(controller.ControllerName)
                : new AuthorizeFilter(PolicyNames.Default));
        }
    }
}