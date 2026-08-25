using Auth.Application.Interfaces;
using Auth.Application.Mappers.Extensions;
using Auth.Domain.Entities;
using Auth.Domain.Interfaces.Services;
using Core.Logging.Decorators;

namespace Auth.Application.Commands;

public record SignInCommand(string Email, string Name, string Password) : Loggable
{
    public object ToLog() => new { Email, Name };
    public object ToLogIdentity() => new { Email };
}

public record SignInOutput(Guid UserId) : Loggable
{
    public object ToLog() => new { UserId };
    public object ToLogIdentity() => new { UserId };
}

public class SignInCommandHandler : IHandler<SignInCommand, Result<SignInOutput>>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public SignInCommandHandler(IUserRepository userRepository, IAuthUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    }

    public async Task<Result<SignInOutput>> HandleAsync(SignInCommand input, CancellationToken ct)
    {
        var userResult = input.ToUser(_passwordHasher);

        if (userResult.IsFailure)
            return userResult.Error;

        var user = userResult.Value;

        if (await _userRepository.IsEmailRegisteredAsync(user.Email, ct))
            return UserErrors.EmailAlreadyExists;

        await _userRepository.CreateUserAsync(user, ct);
        
        var commitResult = await _unitOfWork.CommitAsync([user], ct);

        if (commitResult.IsFailure)
            return commitResult.Error;

        return new SignInOutput(user.Id);
    }
}
