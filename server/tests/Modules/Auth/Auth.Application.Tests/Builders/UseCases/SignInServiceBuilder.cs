using Auth.Application.Tests.Mocks;
using Auth.Domain.Tests.Mocks;
using Auth.Application.UseCases;

namespace Auth.Application.Tests.Builders.UseCases;

public class SignInServiceBuilder
{
    public UserRepositoryMock Repository { get; private set; } = new UserRepositoryMock();
    public AuthUnitOfWorkMock UnitOfWork { get; private set; } = new AuthUnitOfWorkMock();
    public PasswordHasherMock Hasher { get; private set; } = new PasswordHasherMock();

    public SignInServiceBuilder WithRepository(UserRepositoryMock repository) { Repository = repository; return this; }
    public SignInServiceBuilder WithUnitOfWork(AuthUnitOfWorkMock unitOfWork) { UnitOfWork = unitOfWork; return this; }
    public SignInServiceBuilder WithHasher(PasswordHasherMock hasher) { Hasher = hasher; return this; }

    public SignInService Build() => new SignInService(Repository, UnitOfWork, Hasher);
}