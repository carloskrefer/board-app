using Auth.Application.Interfaces;
using Auth.Application.Interfaces.Services;
using Auth.Domain.Entities;
using Auth.Domain.Interfaces.Services;
using Auth.Domain.ValueObjects;

namespace Auth.Application.Commands;

public record LogInCommand(string Email, string Password, string IpAddress, string UserAgent);
public record LogInOutput(string Email, Guid SessionId, string CredentialsSerialized);

public class LogInCommandHandler(
    IUserRepository userRepository, 
    IAuthUnitOfWork unitOfWork, 
    IPasswordHasher passwordHasher,
    ICredentialsService credentialsService) : IHandler<LogInCommand, Result<LogInOutput>>
{
    public async Task<Result<LogInOutput>> HandleAsync(LogInCommand command, CancellationToken ct)
    {
        var email = Email.Create(command.Email);

        if (email.IsFailure)
            return email.Error;

        var password = Password.Create(command.Password);

        if (password.IsFailure)
            return password.Error;

        var user = await userRepository.GetUserByEmailAsync(email.Value, ct);

        if (user is null)
            return UserErrors.EmailNotFound;

        var sessionResult = user.LogIn(command.Password, passwordHasher, command.IpAddress, command.UserAgent);

        if (sessionResult.IsFailure)
            return sessionResult.Error;
        
        var commitResult = await unitOfWork.CommitAsync([user], ct);

        if (commitResult.IsFailure)
            return commitResult.Error;

        var credentials = credentialsService.GenerateSerializedCredentials(user.Id, user.Email.Value, user.Name.Value);

        return new LogInOutput(user.Email.Value, sessionResult.Value.Id, credentials);
    }
}
