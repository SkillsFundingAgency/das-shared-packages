using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace SFA.DAS.Api.Common.Infrastructure
{
    public class ApiExplorerGroupPerVersionConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            var attribute = controller.Attributes.FirstOrDefault(c => c.GetType() == typeof(ApiVersionAttribute)); 
            
            if (attribute != null)
            {
                var apiVersion = ((ApiVersionAttribute) attribute).Versions.FirstOrDefault();
                if (apiVersion != null)
                {
                    controller.ApiExplorer.GroupName = $"v{apiVersion.MajorVersion}";    
                }
            }
            else
            {
                controller.ApiExplorer.GroupName = "v1";    
            }

            if (controller.ControllerName.Equals(PolicyNames.DataLoad))
            {
                controller.ApiExplorer.GroupName = "operations"; 
            }
            
        }
    }
}