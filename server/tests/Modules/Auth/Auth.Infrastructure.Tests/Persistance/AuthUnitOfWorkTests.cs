using Auth.Application.Interfaces;
using Auth.Domain.Entities;
using Auth.Infrastructure.Persistance;
using Auth.Infrastructure.Tests.CollectionFixtures;
using Auth.Infrastructure.Tests.Mocks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestsCommon.Persistance.Base;
using TestsCommon.Persistance.Factories;

namespace Auth.Infrastructure.Tests.Persistance;

[Collection(CollectionFixturesNames.DefaultIntegrationTestsCollection)]
public class AuthUnitOfWorkTests : DefaultIntegrationTests<AuthDbContext>
{   
    public AuthUnitOfWorkTests(DefaultTestingWebApplicationFactory<AuthDbContext> factory) : base(factory) { }


    [Fact]
    public async Task CommitAsync_Should_Return_UpdateConcurrencyError_When_Unexpected_Number_Of_Rows_Is_Changed()
    {   
        using var scopeA = _factory.Services.CreateScope();
        using var scopeB = _factory.Services.CreateScope();
        using var scopeC = _factory.Services.CreateScope();

        var dbA = scopeA.ServiceProvider.GetRequiredService<AuthDbContext>();
        var dbB = scopeB.ServiceProvider.GetRequiredService<AuthDbContext>();
        var dbC = scopeC.ServiceProvider.GetRequiredService<AuthDbContext>();

        var unitA = scopeA.ServiceProvider.GetRequiredService<IAuthUnitOfWork>();
        var unitB = scopeB.ServiceProvider.GetRequiredService<IAuthUnitOfWork>();

        var user = User.Create("email@example.com", "Password123!", "John Doe", new PasswordHasherMock());
        Guid userId = user.Value.Id;
        dbC.Users.Add(user.Value);
        await dbC.SaveChangesAsync();      

        var userA = await dbA.Users.SingleAsync(u => u.Id == userId);
        var userB = await dbB.Users.SingleAsync(u => u.Id == userId);  

        dbA.Entry(userA).Property(u => u.UpdatedAt).IsModified = true;
        dbB.Entry(userB).Property(u => u.UpdatedAt).IsModified = true;

        var resultA = await unitA.CommitAsync([userA], CancellationToken.None);
        var resultB = await unitB.CommitAsync([userB], CancellationToken.None);

        Assert.Equal(1, resultA.Value);
        Assert.Equal(OptimisticUnitOfWorkErrors.UpdateConcurrency, resultB.Error);
    }

    [Fact]
    public async Task CommitAsync_Should_Set_Row_Version_For_Added_And_Updated_Aggregate_Roots()
    {
        using var scopeA = _factory.Services.CreateScope();
        var dbContext = scopeA.ServiceProvider.GetRequiredService<AuthDbContext>();
        var unitOfWork = scopeA.ServiceProvider.GetRequiredService<IAuthUnitOfWork>();

        var result1 = User.Create("email1@example.com", "Password123!", "John Doe", new PasswordHasherMock());   
        var user1 = result1.Value;

        var result2 = User.Create("email2@example.com", "Password123!", "Jane Doe", new PasswordHasherMock());   
        var user2 = result2.Value;

        await unitOfWork.CommitAsync([user1, user2], CancellationToken.None);

        var rowVersion1Before = dbContext.Entry(user1).Property<Guid>("RowVersion").CurrentValue;
        var rowVersion2Before = dbContext.Entry(user2).Property<Guid>("RowVersion").CurrentValue;

        dbContext.Entry(user1).Property(u => u.UpdatedAt).IsModified = true;
        dbContext.Entry(user2).Property(u => u.UpdatedAt).IsModified = true;

        await unitOfWork.CommitAsync([user1, user2], CancellationToken.None);

        var rowVersion1After = dbContext.Entry(user1).Property<Guid>("RowVersion").CurrentValue;
        var rowVersion2After = dbContext.Entry(user2).Property<Guid>("RowVersion").CurrentValue;

        Assert.NotEqual(Guid.Empty, rowVersion1Before);
        Assert.NotEqual(Guid.Empty, rowVersion2Before);
        Assert.NotEqual(Guid.Empty, rowVersion1After);
        Assert.NotEqual(Guid.Empty, rowVersion2After);
        Assert.NotEqual(rowVersion1Before, rowVersion1After);
        Assert.NotEqual(rowVersion2Before, rowVersion2After); 
    }
}