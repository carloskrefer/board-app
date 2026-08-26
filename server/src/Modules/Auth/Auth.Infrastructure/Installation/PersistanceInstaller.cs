using Auth.Application.Interfaces;
using Auth.Infrastructure.Persistance;
using Auth.Infrastructure.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Infrastructure.Installation;

internal static class PersistanceInstaller
{
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