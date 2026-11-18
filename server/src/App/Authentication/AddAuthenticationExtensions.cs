using Microsoft.IdentityModel.Tokens;

namespace App.Authentication;

public static class AddAuthenticationExtensions
{
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