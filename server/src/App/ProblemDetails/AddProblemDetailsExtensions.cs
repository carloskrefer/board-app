using System.Diagnostics;

namespace App.ProblemDetails;

internal static class AddProblemDetailsExtensions
{
    public static void AddProblemDetails(this WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions["traceId"] = Activity.Current?.TraceId.ToString();
                context.ProblemDetails.Instance = context.HttpContext.Request.Path;
                context.ProblemDetails.Extensions["timestamp"] = DateTime.UtcNow; // TODO: Colocar no Model pra aparecer no OpenAPI
            };
        });
    }
}