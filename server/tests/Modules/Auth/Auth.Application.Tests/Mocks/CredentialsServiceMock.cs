using Auth.Application.Interfaces.Services;

namespace Auth.Application.Tests.Mocks;

public class CredentialsServiceMock : ICredentialsService
{
    public Func<Guid, string, string, string> GenerateSerializedCredentialsHandler { get; set; } = 
        (userId, email, name) => $"serialized_credentials_for_{userId}_{email}_{name}";
    public string GenerateSerializedCredentials(Guid userId, string email, string name) => 
        GenerateSerializedCredentialsHandler(userId, email, name);
}