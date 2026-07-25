using Core.Api.DTOs.Auth.User;

namespace Auth.Api.Tests.Builders;

public class SignInRequestBuilder
{
    public string _email { get; private set; } = "test@example.com";
    public string _password { get; private set; } = "Password123!";
    public string _name { get; private set; } = "Joshua";
    
    public SignInRequestBuilder WithEmail(string email) { _email = email; return this; }
    public SignInRequestBuilder WithPassword(string password) { _password = password; return this; }
    public SignInRequestBuilder WithName(string name) { _name = name; return this; }

    public SignInRequest Build() => new SignInRequest(Email: _email, Name: _name, Password: _password);
}