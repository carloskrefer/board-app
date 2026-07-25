using Auth.Application.Interfaces;
using Auth.Application.Interfaces.Services;
using Auth.Domain.Interfaces.Services;
using Auth.Infrastructure.ApplicationServices;
using Auth.Infrastructure.DomainServices;
using Auth.Infrastructure.Persistance;
using Auth.Infrastructure.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Infrastructure.Installation.Extensions;

internal static class IServiceCollectionExtensions
{

    public static void AddApplicationServicesImplementations(this IServiceCollection services)
    {
        services.AddScoped<ICredentialsService, JwtService>();
    } 


    public static void AddDomainServicesImplementations(this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher, PasswordHasher>();
    }

    internal static void AddAuthDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default");

        services.AddDbContext<AuthDbContext>(options => 
        {
            options
                .UseNpgsql(connectionString, npgsqlOptions =>
                {
                    npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, AuthDbContext.Schema);
                })
                .UseSnakeCaseNamingConvention();
        });
    }

    internal static void AddUnitOfWork(this IServiceCollection services) => 
        services.AddScoped<IAuthUnitOfWork>(sp => 
        {
            return sp.GetRequiredService<AuthDbContext>()
                ?? throw new InvalidOperationException($"Failed to resolve AuthDbContext from the service provider.");

        });

    internal static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IUserRepository, UserRepository>();
    }
}