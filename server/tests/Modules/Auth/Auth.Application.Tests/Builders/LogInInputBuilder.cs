using Auth.Application.Commands;

namespace Auth.Application.Tests.Builders;

public class LogInCommandBuilder
{
    private string _email = "joshua@example.com";
    private string _password = "Senha@123";
    private string _ipAddress = "127.0.0.1";
    private string _userAgent = "Test User Agent";

    public LogInCommandBuilder WithEmail(string email) { _email = email; return this; }

    public LogInCommandBuilder WithPassword(string password) { _password = password; return this; }

    public LogInCommandBuilder WithIpAddress(string ipAddress) { _ipAddress = ipAddress; return this; }

    public LogInCommandBuilder WithUserAgent(string userAgent) { _userAgent = userAgent; return this; }

    public LogInCommand Build()
    {
        return new LogInCommand(_email, _password, _ipAddress, _userAgent);
    }
}