using Auth.Domain.Entities;
using Auth.Domain.Tests.Mocks;

namespace Auth.Application.Tests.Builders.Domain.Entities;

public class UserBuilder
{
    public string _email { get; private set; } = "joshua@example.com";
    public string _password { get; private set; } = "Senha@123";
    public string _name { get; private set; } = "Joshua";
    public PasswordHasherMock _passwordHasher { get; private set; } = new PasswordHasherMock();

    public UserBuilder WithEmail(string email) { _email = email; return this; }
    public UserBuilder WithPassword(string password) { _password = password; return this; }
    public UserBuilder WithName(string name) { _name = name; return this; }
    public UserBuilder WithPasswordHasher(PasswordHasherMock hasher) { _passwordHasher = hasher; return this; }

    public User Build()
    {
        var userResult = User.Create(_email, _password, _name, _passwordHasher);
        if (userResult.IsFailure)
            throw new InvalidOperationException($"Failed to create User: {userResult.Error}");

        return userResult.Value;
    }

}