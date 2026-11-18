using Microsoft.AspNetCore.Cors.Infrastructure;

namespace App.Cors;

public static class AddCorsExtensions 
{
    public static void AddCors(this WebApplicationBuilder builder) =>
        builder.Services.AddCors(AddDevelopmentPolicy);

    private static void AddDevelopmentPolicy(CorsOptions options) =>
        options.AddPolicy(PolicyNames.Development, ConfigureDevelopmentPolicy);

    private static void ConfigureDevelopmentPolicy(CorsPolicyBuilder policy) =>
        policy
            .WithOrigins("http://localhost:4200") // TODO: Move this string to appsettings
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
}