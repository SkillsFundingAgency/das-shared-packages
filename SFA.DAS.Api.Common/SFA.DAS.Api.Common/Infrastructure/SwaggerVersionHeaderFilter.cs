using System.Collections.Generic;
using System.Text.Json.Nodes;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SFA.DAS.Api.Common.Infrastructure
{
    public class SwaggerVersionHeaderFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            operation.Parameters ??= new List<IOpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "X-Version",
                In = ParameterLocation.Header,
                AllowEmptyValue = false,
                Example =  JsonNode.Parse("1.0"),
                Required = true 
            });
        }
    }
}