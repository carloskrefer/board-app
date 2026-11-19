using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;

namespace App.Authentication;

public static class AddAuthenticationExtensions
{
    internal static void AddAuthentication(this WebApplicationBuilder builder)
    {
        builder.Services.AddAuthentication()
            .AddJwtBearer(options => ConfigureJwtBearerOptions(options, builder));

        builder.Services.AddAuthorization(ConfigureAuthorizationOptions);
    }

    private static void ConfigureJwtBearerOptions(JwtBearerOptions options, WebApplicationBuilder builder)
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
    }

    private static void ConfigureAuthorizationOptions(AuthorizationOptions options)
    {
        options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .Build();
    }
}