using Auth.Domain.Interfaces.Services;

namespace Auth.Infrastructure.DomainServices;

public class PasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<object>();
        return passwordHasher.HashPassword(new object(), password);
    }

    public bool Verify(string passwordText, string passwordHashed)
    {
        var passwordHasher = new Microsoft.AspNetCore.Identity.PasswordHasher<object>();
        var result = passwordHasher.VerifyHashedPassword(new object(), passwordHashed, passwordText);
        return result == Microsoft.AspNetCore.Identity.PasswordVerificationResult.Success;
    }
}