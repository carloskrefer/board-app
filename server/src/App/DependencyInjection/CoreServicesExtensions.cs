using Core.Api;

namespace App.DependencyInjection;

internal static class CoreServicesExtensions
{
    public static void AddCoreServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<ProblemResultFactory2>();
    }
}