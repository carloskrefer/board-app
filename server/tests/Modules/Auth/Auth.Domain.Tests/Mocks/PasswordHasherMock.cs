using Auth.Domain.Interfaces.Services;

namespace Auth.Domain.Tests.Mocks;

public class PasswordHasherMock : IPasswordHasher
{
    public Func<string, string> HashHandler { get; set; } = 
        password => $"{password}_hashed";
    public string Hash(string password) => HashHandler(password);

    public Func<string, string, bool> VerifyHandler { get; set; } = 
        (password, passwordHashed) => $"{password}_hashed" == passwordHashed;
    public bool Verify(string password, string passwordHashed) => VerifyHandler(password, passwordHashed);
}