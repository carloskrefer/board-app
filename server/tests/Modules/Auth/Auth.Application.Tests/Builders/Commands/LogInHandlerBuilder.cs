using Auth.Application.Tests.Mocks;
using Auth.Domain.Tests.Mocks;
using Auth.Application.Commands;

namespace Auth.Application.Tests.Builders.Commands;

public class LogInCommandHandlerBuilder
{
    public UserRepositoryMock Repository { get; private set; } = new UserRepositoryMock();
    public AuthUnitOfWorkMock UnitOfWork { get; private set; } = new AuthUnitOfWorkMock();
    public PasswordHasherMock Hasher { get; private set; } = new PasswordHasherMock();
    public CredentialsServiceMock CredentialsService { get; private set; } = new CredentialsServiceMock();

    public LogInCommandHandlerBuilder WithRepository(UserRepositoryMock repository) { Repository = repository; return this; }
    public LogInCommandHandlerBuilder WithUnitOfWork(AuthUnitOfWorkMock unitOfWork) { UnitOfWork = unitOfWork; return this; }
    public LogInCommandHandlerBuilder WithPasswordHasher(PasswordHasherMock hasher) { Hasher = hasher; return this; }
    public LogInCommandHandlerBuilder WithCredentialsService(CredentialsServiceMock credentialsService) { CredentialsService = credentialsService; return this; }

    public LogInCommandHandler Build() => new LogInCommandHandler(Repository, UnitOfWork, Hasher, CredentialsService);
}