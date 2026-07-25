using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;
using Auth.Infrastructure.Persistance.EntityConfigurations;
using Auth.Infrastructure.Persistance.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Persistance.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _dbContext;

    public UserRepository(AuthDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }

    public async Task CreateUserAsync(User user, CancellationToken ct)
    {
        await _dbContext.Users.AddAsync(user, ct);
    }

    public async Task<User?> GetUserByEmailAsync(Email email, CancellationToken ct)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email.Value == email.Value, ct);
    }

    public async Task<bool> IsEmailRegisteredAsync(Email email, CancellationToken ct)
    {
        return await _dbContext.Users.AnyAsync(u => u.Email.Value == email.Value, ct);
    }
}