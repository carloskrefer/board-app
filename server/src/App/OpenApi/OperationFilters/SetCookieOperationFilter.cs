using Core.Api.OpenApi.Attributes;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace App.OpenApi.OperationFilters;

public class SetCookieOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var attrs = context.MethodInfo
            .GetCustomAttributes(typeof(SetCookieAttribute), inherit: true)
            .Cast<SetCookieAttribute>();

        foreach (var attr in attrs)
        {
            if (!operation.Responses.TryGetValue(attr.WhenStatusCode.ToString(), out var response))
                continue;

            response.Headers[attr.GetHeader()] = new OpenApiHeader
            {
                Description = attr.Description,
                Schema = new OpenApiSchema { Type = "string" },
                Required = attr.Required,
                Deprecated = attr.Deprecated
            };
        }
    }
}