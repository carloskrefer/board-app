using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TestCommon.Persistance.Extensions;

public static class IServiceCollectionExtensions
{
    internal static void ApplyMigrations<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
    {
        var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<TDbContext>();
        db.Database.Migrate();
    }
}