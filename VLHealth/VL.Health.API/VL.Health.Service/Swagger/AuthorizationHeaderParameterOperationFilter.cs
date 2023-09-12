using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Collections.Generic;
using System.Linq;

namespace VL.Health.Service.Swagger
{
    public class AuthorizationHeaderParameterOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var filterPipeline = context.ApiDescription.ActionDescriptor.FilterDescriptors;
            var notContainsAllowAnonymous = !filterPipeline.Any(filter => filter.Filter is IAllowAnonymousFilter);

            if (notContainsAllowAnonymous)
            {               
                operation.Parameters.Add(CreateNonBodyParameter("X-Tenant-Id", "Tenant client"));
            }
        }
        private OpenApiParameter CreateNonBodyParameter(string name, string description)
        {
            return new OpenApiParameter
            { 
                Name = name,               
                In = ParameterLocation.Header,
                Description = description,
                Required = true,
                Schema = new OpenApiSchema
                {
                    Type = "string"
                }
            };
        }
    }
}
