using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;
using TestsCommon.Mocks.Helpers;

namespace Auth.Application.Tests.Mocks;

public class UserRepositoryMock : IUserRepository
{
    public CallTracker Tracker { get; } = new();

    public Func<User, CancellationToken, Task> CreateUserAsyncHandler { get; set; } = (_, _) => Task.CompletedTask; 
    public async Task CreateUserAsync(User user, CancellationToken ct)
    {
        Tracker.Record([user, ct]);
        await CreateUserAsyncHandler(user, ct);
    }

    public Func<Email, CancellationToken, Task<User?>> GetUserByEmailAsyncHandler { get; set; } = 
        (_, _) => Task.FromResult<User?>(null);
    public Task<User?> GetUserByEmailAsync(Email email, CancellationToken ct) {
        Tracker.Record([email, ct]);
        return GetUserByEmailAsyncHandler(email, ct);
    }   

    public Func<Email, CancellationToken, Task<bool>> IsEmailRegisteredAsyncHandler { get; set; } = 
        (_, _) => Task.FromResult(false);
    public Task<bool> IsEmailRegisteredAsync(Email email, CancellationToken ct)
    {   
        Tracker.Record([email, ct]);
        return IsEmailRegisteredAsyncHandler(email, ct);
    }
}