namespace Auth.Domain.Interfaces.Services;

public interface IPasswordHasher
{
    public string Hash(string password);
    public bool Verify(string passwordText, string passwordHashed);
}