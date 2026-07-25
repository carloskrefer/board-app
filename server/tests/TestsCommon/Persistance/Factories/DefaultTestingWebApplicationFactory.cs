using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestCommon.Persistance.Extensions;
using TestsCommon.Persistance.Interfaces;
using TestsCommon.Services.Services;

namespace TestsCommon.Persistance.Factories;

public class DefaultTestingWebApplicationFactory<TDbContext> : WebApplicationFactory<Program> 
    where TDbContext : DbContext
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices((context, services) =>
        {
            services.AddScoped<IDatabaseSpecificCommandsService, PostgresCommandsService<TDbContext>>();
            services.ApplyMigrations<TDbContext>();
        });
    }
}