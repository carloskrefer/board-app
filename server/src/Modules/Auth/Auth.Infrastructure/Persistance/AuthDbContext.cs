
using Auth.Application.Interfaces;
using Auth.Domain.Entities;
using Auth.Infrastructure.Persistance.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Persistance;

public class AuthDbContext : DbContext, IAuthUnitOfWork
{
    public const string Schema = "auth";

    public DbSet<User> Users { get; set; }
    public DbSet<Session> Sessions { get; set; }

    public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder) 
    {
        modelBuilder.HasDefaultSchema(Schema);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AuthDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    public async Task<Result<int>> CommitAsync(IEnumerable<object> aggregateRootsTouched, CancellationToken ct)
    {
        OptimisticConcurrencyHelper.UpdateManyRowVersions(aggregateRootsTouched, this);
        AddNewEntities(aggregateRootsTouched);

        try
        {
            return await base.SaveChangesAsync(ct);
        }
        catch (DbUpdateConcurrencyException)
        {
            return UnitOfWorkErrors.OptimisticConcurrency;
        }
    }

    private void AddNewEntities(IEnumerable<object> aggregateRootsTouched)
    {
        foreach (var entity in aggregateRootsTouched)
        {
            if (entity is User user)
            {
                AddRange(user.NewSessions);
                user.NewSessions.Clear();
            }
        }
    }
}