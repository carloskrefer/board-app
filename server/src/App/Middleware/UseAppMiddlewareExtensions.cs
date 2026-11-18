using App.Cors;
using App.OpenApi;
using Board.App.Tracking;

namespace App.Middleware;

internal static class UseAppMiddlewareExtensions
{
    public static void UseAppMiddleware(this WebApplication app)
    {
        app.UseSwaggerMiddleware();
        app.UseCorsMiddleware();
        app.UseMiddleware<TraceIdHeaderMiddleware>();
        app.UseExceptionHandler(); // TODO: Write test for this
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
    }
}