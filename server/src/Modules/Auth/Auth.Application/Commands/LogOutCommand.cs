using Auth.Application.Interfaces;
using Auth.Domain.Entities;

namespace Auth.Application.Commands;

public record LogOutCommand(Guid UserId, Guid SessionId);
public record LogOutOutput();

public interface ILogOutCommandHandler : IHandler<LogOutCommand, Result<LogOutOutput>> { }

public class LogOutCommandHandler(
    IUserRepository userRepository,
    IAuthUnitOfWork unitOfWork) : ILogOutCommandHandler
{
    public async Task<Result<LogOutOutput>> HandleAsync(LogOutCommand command, CancellationToken ct)
    {
        var user = await userRepository.GetUserByIdAsync(command.UserId, ct);

        if (user is null)
            return UserErrors.IdNotFound;

        user.LogOut(command.SessionId);

        var result = await unitOfWork.CommitAsync([user], ct);

        if (result.IsFailure)
            return result.Error;

        return new LogOutOutput();
    }
}
