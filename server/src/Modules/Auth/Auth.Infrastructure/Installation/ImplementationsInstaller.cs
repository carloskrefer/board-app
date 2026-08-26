using Auth.Application.Interfaces.Services;
using Auth.Domain.Interfaces.Services;
using Auth.Infrastructure.Jwt;
using Auth.Infrastructure.Hash;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Infrastructure.Installation;

internal static class ImplementationsInstaller
{
    public static void AddApplicationServicesImplementations(this IServiceCollection services)
    {
        services.AddScoped<ICredentialsService, JwtService>();
    } 


    public static void AddDomainServicesImplementations(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher, PasswordHasher>();
    }
}