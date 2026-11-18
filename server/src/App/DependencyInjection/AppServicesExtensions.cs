using App.Authentication;
using App.Cors;
using App.Mvc;
using App.OpenApi;
using App.ProblemDetails;
using Board.App.Exception;

namespace App.DependencyInjection;

internal static class DependencyInjectionExtensions
{
    public static void AddAppServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddHttpClient();
        builder.AddAuthentication();
        builder.AddModules();
        builder.AddMvcServices();
        builder.AddSwaggerServices();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        builder.AddProblemDetails();
        builder.Services.AddHttpContextAccessor();
        builder.AddCoreServices();
        builder.AddCors();
    }
}