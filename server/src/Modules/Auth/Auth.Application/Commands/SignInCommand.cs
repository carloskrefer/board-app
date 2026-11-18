using Auth.Application.Interfaces;
using Auth.Application.Mappers.Extensions;
using Auth.Domain.Entities;
using Auth.Domain.Interfaces.Services;

namespace Auth.Application.Commands;

public record SignInCommand(string Email, string Name, string Password);
public record SignInOutput(Guid UserId);

public class SignInCommandHandler(
    IUserRepository userRepository,
    IAuthUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher) : IHandler<SignInCommand, Result<SignInOutput>>
{
    public async Task<Result<SignInOutput>> HandleAsync(SignInCommand command, CancellationToken ct)
    {
        var userResult = command.ToUser(passwordHasher);

        if (userResult.IsFailure)
            return userResult.Error;

        var user = userResult.Value;

        if (await userRepository.IsEmailRegisteredAsync(user.Email, ct))
            return UserErrors.EmailAlreadyExists;

        await userRepository.CreateUserAsync(user, ct);
        
        var commitResult = await unitOfWork.CommitAsync([user], ct);

        if (commitResult.IsFailure)
            return commitResult.Error;

        return new SignInOutput(user.Id);
    }
}
