using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;

public interface IUserRepository
{
    public Task CreateUserAsync(User user, CancellationToken ct);
    public Task<User?> GetUserByEmailAsync(Email email, CancellationToken ct);
    public Task<bool> IsEmailRegisteredAsync(Email email, CancellationToken ct);
}