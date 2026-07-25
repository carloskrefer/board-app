using Auth.Domain.Entities;
using Auth.Domain.Interfaces.Services;
using Auth.Domain.Tests.Mocks;

namespace Auth.Domain.Tests.Builders;

internal class UserBuilder
{
    private string _email = "joshua@example.com";
    public string _password { get; private set; } = "Senha@123";
    private string _name = "Joshua";
    public IPasswordHasher _passwordHasher { get; private set; } = new PasswordHasherMock();

    public UserBuilder WithEmail(string email) { _email = email; return this; }
    public UserBuilder WithPassword(string password) { _password = password; return this; }
    public UserBuilder WithName(string name) { _name = name; return this; }
    public UserBuilder WithPasswordHasher(IPasswordHasher passwordHasher) { _passwordHasher = passwordHasher; return this; }

    public User Build()
    {
        var result = User.Create(_email, _password, _name, _passwordHasher);

        if (result.IsFailure)
            throw new InvalidOperationException(
                $"Failed to create user. [{result.Error.Code}] {result.Error.Description}");

        return result.Value;
    }
}