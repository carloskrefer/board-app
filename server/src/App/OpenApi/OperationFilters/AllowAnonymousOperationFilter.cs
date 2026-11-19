using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace App.OpenApi.OperationFilters;

public class AllowAnonymousOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var metadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;
        var emptySecurityRequirement = new OpenApiSecurityRequirement();

        if (metadata.OfType<IAllowAnonymous>().Any())
            operation.Security = [emptySecurityRequirement];
    }
}