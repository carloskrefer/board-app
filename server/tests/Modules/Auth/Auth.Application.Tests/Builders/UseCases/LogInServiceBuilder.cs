using Auth.Application.Tests.Mocks;
using Auth.Domain.Tests.Mocks;
using Auth.Application.UseCases;

namespace Auth.Application.Tests.Builders.UseCases;

public class LogInServiceBuilder
{
    public UserRepositoryMock Repository { get; private set; } = new UserRepositoryMock();
    public AuthUnitOfWorkMock UnitOfWork { get; private set; } = new AuthUnitOfWorkMock();
    public PasswordHasherMock Hasher { get; private set; } = new PasswordHasherMock();
    public CredentialsServiceMock CredentialsService { get; private set; } = new CredentialsServiceMock();

    public LogInServiceBuilder WithRepository(UserRepositoryMock repository) { Repository = repository; return this; }
    public LogInServiceBuilder WithUnitOfWork(AuthUnitOfWorkMock unitOfWork) { UnitOfWork = unitOfWork; return this; }
    public LogInServiceBuilder WithPasswordHasher(PasswordHasherMock hasher) { Hasher = hasher; return this; }
    public LogInServiceBuilder WithCredentialsService(CredentialsServiceMock credentialsService) { CredentialsService = credentialsService; return this; }

    public LogInService Build() => new LogInService(Repository, UnitOfWork, Hasher, CredentialsService);
}