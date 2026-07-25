using Auth.Domain.Interfaces.Services;
using Auth.Domain.ValueObjects;

namespace Auth.Domain.Entities;

public class User
{
    public Guid Id { get; private set; }
    public Email Email { get; private set; } = null!;
    public string PasswordHash { get; private set; } = null!;
    public Name Name { get; private set; } = null!;
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    private List<Session> _sessions = [];
    public IReadOnlyCollection<Session> Sessions => _sessions.AsReadOnly();
    public List<Session> NewSessions { get; set; } = [];

    public static Result<User> Create(string email, string password, string name, IPasswordHasher hasher)
    {
        var emailResult = Email.Create(email);

        if (emailResult.IsFailure)
            return emailResult.Error;

        var nameResult = Name.Create(name);

        if (nameResult.IsFailure)
            return nameResult.Error;

        var passwordResult = Password.Create(password);

        if (passwordResult.IsFailure)
            return passwordResult.Error;

        return new User
        {
            Id = Guid.NewGuid(),
            Email = emailResult.Value,
            PasswordHash = hasher.Hash(passwordResult.Value.Value),
            Name = nameResult.Value,
            CreatedAt = DateTime.UtcNow
        };
    }

    public Result<Session> LogIn(string password, IPasswordHasher passwordHasher, string ipAddress, string userAgent)
    {
        if (!passwordHasher.Verify(password, PasswordHash))
            return UserErrors.IncorrectPassword;

        var session = Session.Create(this, ipAddress, userAgent);

        if (session.IsFailure)
            return session.Error;

        _sessions.Add(session.Value);
        NewSessions.Add(session.Value);

        return session;
    }
}

public static class UserErrors
{
    public static readonly string Id = "USER";
    public static readonly Error EmailAlreadyExists = 
        new(
            $"{Id}.EMAIL_ALREADY_EXISTS", 
            "An user with the provided email already exists.");

    public static readonly Error NotFound = 
        new(
            $"{Id}.USER_NOT_FOUND", 
            "An user with the provided email was not found.");

    public static readonly Error IncorrectPassword = // TODO: Rename to PasswordIncorrect
        new(
            $"{Id}.INCORRECT_PASSWORD", 
            "The provided password is incorrect.");
}