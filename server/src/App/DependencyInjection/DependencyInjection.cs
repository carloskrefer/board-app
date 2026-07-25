

using App.DependencyInjection.Extensions;
using Auth.Infrastructure;
using Microsoft.IdentityModel.Tokens;

namespace App.DependencyInjection;

public static class DependencyInjection
{
    public static void AddModules(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthModule(builder.Configuration);
    }

    internal static void AddAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication()
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = builder.Configuration.GetJwtIssuer(),

                    ValidateAudience = true,
                    ValidAudience = builder.Configuration.GetJwtAudience(),

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = builder.Configuration.GetRsaSecurityKey(),

                    ValidateLifetime = true

                    // TODO: Tem outras configurações importantes de setar aqui, conferir e setar!
                };
            });
    }
}