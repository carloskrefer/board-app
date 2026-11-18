using Auth.Infrastructure;

namespace App.DependencyInjection;

internal static class ModulesServicesExtensions
{
    public static void AddModules(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthModule(builder.Configuration);
    }
}