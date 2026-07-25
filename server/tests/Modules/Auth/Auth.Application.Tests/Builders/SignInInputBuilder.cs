using Auth.Application.UseCases;

namespace Auth.Application.Tests.Builders;

public class SignInInputBuilder
{
    private string _email = "joshua@example.com";
    private string _password = "Senha@123";
    private string _name = "Joshua";

    public SignInInputBuilder WithEmail(string email) { _email = email; return this; }

    public SignInInputBuilder WithPassword(string password) { _password = password; return this; }

    public SignInInputBuilder WithName(string name) { _name = name; return this; }

    public SignInInput Build()
    {
        return new SignInInput(_email, _name, _password);
    }
}