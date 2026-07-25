namespace Auth.Domain.Entities;

public class Session
{
    public Guid Id { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public DateTime? RevokedAt { get; private set; }
    public string IpAddress { get; private set; } = null!;
    public string UserAgent { get; private set; } = null!;
    
    public Guid UserId { get; private set; }
    public User? User { get; private set; }

    internal static Result<Session> Create(User user, string ipAddress, string userAgent)
    {
        return new Session
        {
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7), // TODO: Make this configurable
            IpAddress = ipAddress,
            UserAgent = userAgent,
            UserId = user.Id,
            User = user
        };
    }
}
