using Auth.Application.Commands;

namespace Auth.Application.Tests.Builders;

public class SignInCommandBuilder
{
    private string _email = "joshua@example.com";
    private string _password = "Senha@123";
    private string _name = "Joshua";

    public SignInCommandBuilder WithEmail(string email) { _email = email; return this; }

    public SignInCommandBuilder WithPassword(string password) { _password = password; return this; }

    public SignInCommandBuilder WithName(string name) { _name = name; return this; }

    public SignInCommand Build()
    {
        return new SignInCommand(_email, _name, _password);
    }
}