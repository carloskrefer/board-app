using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestsCommon.Persistance.Factories;
using TestsCommon.Persistance.Interfaces;

namespace TestsCommon.Persistance.Base;

public class DefaultIntegrationTests<TDbContext> : IAsyncLifetime where TDbContext : DbContext
{
    protected readonly DefaultTestingWebApplicationFactory<TDbContext> _factory;
    protected readonly HttpClient _client;

    public DefaultIntegrationTests(DefaultTestingWebApplicationFactory<TDbContext> factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    public async Task InitializeAsync()
    {
        using var scope = _factory.Services.CreateScope();
        var databaseService = scope.ServiceProvider.GetRequiredService<IDatabaseSpecificCommandsService>();
        await databaseService.TruncateAllTables();
    }

    public async Task DisposeAsync() { }
}