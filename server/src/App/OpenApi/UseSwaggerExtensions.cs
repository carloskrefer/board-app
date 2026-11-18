using Core.Api.OpenApi;

namespace App.OpenApi;

internal static class UseSwaggerExtensions
{
    public static void UseSwaggerMiddleware(this WebApplication app)
    {
        app.UseSwagger(options => options.RouteTemplate = "swagger/{documentName}/swagger.json");
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint($"/swagger/{Versions.Version1}/swagger.json", "Current API");
            options.EnableDeepLinking();
            options.DefaultModelsExpandDepth(0);
            options.DisplayRequestDuration();
        });
    }
}