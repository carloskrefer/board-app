using Auth.Application.Interfaces;
using Auth.Application.Interfaces.Services;
using Auth.Domain.Entities;
using Auth.Domain.Interfaces.Services;
using Auth.Domain.ValueObjects;
using Core.Logging.Decorators;

namespace Auth.Application.UseCases;

public record LogInInput(string Email, string Password, string IpAddress, string UserAgent) : Loggable
{
    public object ToLog() => new { Email };
    public object ToLogIdentity() => new { Email };
}

public record LogInOutput(string Email, Guid SessionId, string CredentialsSerialized) : Loggable
{
    public object ToLog() => new { Email };
    public object ToLogIdentity() => new { Email };
}

public class LogInService : IUseCase<LogInInput, Result<LogInOutput>>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ICredentialsService _credentialsService;

    public LogInService(
        IUserRepository userRepository, 
        IAuthUnitOfWork unitOfWork, 
        IPasswordHasher passwordHasher,
        ICredentialsService credentialsService)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _credentialsService = credentialsService ?? throw new ArgumentNullException(nameof(credentialsService));
    }

    public async Task<Result<LogInOutput>> ExecuteAsync(LogInInput input, CancellationToken ct)
    {
        var email = Email.Create(input.Email);

        if (email.IsFailure)
            return email.Error;

        var password = Password.Create(input.Password);

        if (password.IsFailure)
            return password.Error;

        var user = await _userRepository.GetUserByEmailAsync(email.Value, ct);

        if (user is null)
            return UserErrors.NotFound;

        var sessionResult = user.LogIn(input.Password, _passwordHasher, input.IpAddress, input.UserAgent);

        if (sessionResult.IsFailure)
            return sessionResult.Error;
        
        var commitResult = await _unitOfWork.CommitAsync([user], ct);

        if (commitResult.IsFailure)
            return commitResult.Error;

        var credentials = _credentialsService.GenerateSerializedCredentials(user.Id, user.Email.Value, user.Name.Value);

        return new LogInOutput(user.Email.Value, sessionResult.Value.Id, credentials);
    }
}
