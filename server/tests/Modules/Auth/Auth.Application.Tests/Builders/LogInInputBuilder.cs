using Auth.Application.UseCases;

namespace Auth.Application.Tests.Builders;

public class LogInInputBuilder
{
    private string _email = "joshua@example.com";
    private string _password = "Senha@123";
    private string _ipAddress = "127.0.0.1";
    private string _userAgent = "Test User Agent";

    public LogInInputBuilder WithEmail(string email) { _email = email; return this; }

    public LogInInputBuilder WithPassword(string password) { _password = password; return this; }

    public LogInInputBuilder WithIpAddress(string ipAddress) { _ipAddress = ipAddress; return this; }

    public LogInInputBuilder WithUserAgent(string userAgent) { _userAgent = userAgent; return this; }

    public LogInInput Build()
    {
        return new LogInInput(_email, _password, _ipAddress, _userAgent);
    }
}