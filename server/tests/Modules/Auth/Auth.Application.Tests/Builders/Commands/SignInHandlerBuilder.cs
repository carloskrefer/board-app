using Auth.Application.Tests.Mocks;
using Auth.Domain.Tests.Mocks;
using Auth.Application.Commands;

namespace Auth.Application.Tests.Builders.Commands;

public class SignInCommandHandlerBuilder
{
    public UserRepositoryMock Repository { get; private set; } = new UserRepositoryMock();
    public AuthUnitOfWorkMock UnitOfWork { get; private set; } = new AuthUnitOfWorkMock();
    public PasswordHasherMock Hasher { get; private set; } = new PasswordHasherMock();

    public SignInCommandHandlerBuilder WithRepository(UserRepositoryMock repository) { Repository = repository; return this; }
    public SignInCommandHandlerBuilder WithUnitOfWork(AuthUnitOfWorkMock unitOfWork) { UnitOfWork = unitOfWork; return this; }
    public SignInCommandHandlerBuilder WithHasher(PasswordHasherMock hasher) { Hasher = hasher; return this; }

    public SignInCommandHandler Build() => new SignInCommandHandler(Repository, UnitOfWork, Hasher);
}