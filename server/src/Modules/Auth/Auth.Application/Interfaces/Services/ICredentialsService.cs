namespace Auth.Application.Interfaces.Services;

public interface ICredentialsService
{
    public string GenerateSerializedCredentials(Guid userId, string email, string name);
}