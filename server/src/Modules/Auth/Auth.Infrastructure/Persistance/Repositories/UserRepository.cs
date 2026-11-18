using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace Auth.Infrastructure.Persistance.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AuthDbContext _dbContext;
    private IQueryable<User> _usersFullyIncluded;

    public UserRepository(AuthDbContext dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _usersFullyIncluded = _dbContext.Users.Include(u => u.Sessions);
    }

    public async Task CreateUserAsync(User user, CancellationToken ct)
    {
        await _dbContext.AddAsync(user, ct);
    }

    public async Task<User?> GetUserByEmailAsync(Email email, CancellationToken ct)
    {
        return await _usersFullyIncluded.FirstOrDefaultAsync(u => u.Email.Value == email.Value, ct);
    }

    public async Task<bool> IsEmailRegisteredAsync(Email email, CancellationToken ct)
    {
        return await _dbContext.Users.AsNoTracking().AnyAsync(u => u.Email.Value == email.Value, ct);
    }

    public async Task<User?> GetUserByIdAsync(Guid userId, CancellationToken ct)
    {
        return await _usersFullyIncluded.FirstOrDefaultAsync(u => u.Id == userId, ct);
    }
}