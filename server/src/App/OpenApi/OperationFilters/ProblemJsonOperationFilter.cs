using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace App.OpenApi.OperationFilters;

public class ProblemJsonOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        foreach (var (code, response) in operation.Responses)
        {
            if (!int.TryParse(code, out var status) || status < 400) continue;

            if (response.Content.TryGetValue("application/json", out var media))
            {
                response.Content.Clear();
                response.Content["application/problem+json"] = media;
            }
        }
    }
}