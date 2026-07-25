using Microsoft.EntityFrameworkCore;
using TestsCommon.Persistance.Interfaces;

namespace TestsCommon.Services.Services;

public class PostgresCommandsService<TDbContext> : IDatabaseSpecificCommandsService
    where TDbContext : DbContext
{
    private readonly TDbContext _dbContext;

    public PostgresCommandsService(TDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task TruncateAllTables()
    {
        var tablesNames = _dbContext.Model
            .GetEntityTypes()
            .Where(e => e.GetTableName() is not null)
            .Select(e => $"{e.GetSchema()}.{e.GetTableName()}")
            .Distinct();

        var sql =
            $"""
            TRUNCATE TABLE
            {string.Join(", ", tablesNames)}
            RESTART IDENTITY CASCADE;
            """;

        await _dbContext.Database.ExecuteSqlRawAsync(sql);
    }
}