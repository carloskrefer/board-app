using Auth.Application.Installation.Extensions;
using Auth.Infrastructure.Installation.Extensions;
using Auth.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Auth.Infrastructure;

public static class AuthModuleInstaller
{
    public static void AddAuthModule(this IServiceCollection services, IConfiguration configuration)
    {   
        services.AddAuthDbContext(configuration);
        services.AddUnitOfWork();
        services.AddRepositories();
        services.AddApplicationServicesImplementations();
        services.AddApplicationServices();
        services.AddDomainServicesImplementations();
    }

    public static async Task ApplyAuthModuleMigrationsAsync(this IServiceProvider service)
    {
        using var scope = service.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        await dbContext.Database.MigrateAsync();
    }
}