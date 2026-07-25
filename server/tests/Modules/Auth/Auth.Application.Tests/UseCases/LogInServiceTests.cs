using Auth.Application.Tests.Builders;
using Auth.Application.Tests.Mocks;
using Auth.Domain.Entities;
using Auth.Application.Tests.Builders.UseCases;
using Auth.Application.Tests.Builders.Domain.Entities;
using Auth.Application.UseCases;
using Auth.Domain.ValueObjects;

namespace Auth.Application.Tests.UseCases;

public class LogInServiceTests
{
    [Fact]
    public async Task Should_Create_And_Return_Single_Correct_Session_When_Input_Is_Valid()
    {
        var builder = new LogInServiceBuilder();
        var user = new UserBuilder().WithPasswordHasher(builder.Hasher).Build();

        UserRepositoryMock repository = new() {
            GetUserByEmailAsyncHandler = (email, _) =>
                email.Equals(user.Email) ? Task.FromResult<User?>(user) : Task.FromResult<User?>(null)
        };

        var service = builder.WithRepository(repository).Build();
        var input = new LogInInputBuilder().WithEmail(user.Email.Value).Build();

        var result = await service.ExecuteAsync(input, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Single(user.Sessions);
        Assert.Equal(user.Sessions.First().Id, result.Value.SessionId);
    }

    [Fact]
    public async Task Should_Generate_And_Return_Correct_Credentials_When_Input_Is_Valid()
    {
        var builder = new LogInServiceBuilder();
        var user = new UserBuilder().WithPasswordHasher(builder.Hasher).Build();

        UserRepositoryMock repository = new() {
            GetUserByEmailAsyncHandler = (email, _) =>
                email.Equals(user.Email) ? Task.FromResult<User?>(user) : Task.FromResult<User?>(null)
        };

        var service = builder.WithRepository(repository).Build();
        
        var expectedCredentials = builder.CredentialsService.GenerateSerializedCredentials(
            user.Id, 
            user.Email.Value, 
            user.Name.Value);

        var input = new LogInInputBuilder().WithEmail(user.Email.Value).Build();

        var result = await service.ExecuteAsync(input, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(expectedCredentials, result.Value.CredentialsSerialized);
    }

    [Fact]
    public async Task Should_Only_Call_GetUserByEmailAsync_And_CommitAsync_Once_When_Input_Is_Valid()
    {
        var builder = new LogInServiceBuilder();
        var unitOfWork = builder.UnitOfWork;

        UserRepositoryMock repository = new() {
            GetUserByEmailAsyncHandler = (_, _) => Task.FromResult<User?>(new UserBuilder().Build())
        };

        var service = builder.WithRepository(repository).Build();
        var input = new LogInInputBuilder().Build();

        var result = await service.ExecuteAsync(input, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Equal(1, repository.Tracker.GetCallCount(nameof(repository.GetUserByEmailAsync)));
        Assert.Equal(1, repository.Tracker.TotalCalls);
        Assert.Equal(1, unitOfWork.Tracker.GetCallCount(nameof(unitOfWork.CommitAsync)));
        Assert.Equal(1, unitOfWork.Tracker.TotalCalls);
    }

    [Fact]
    public async Task Should_Return_NotFound_Error_When_Email_Does_Not_Exist()
    {
        UserRepositoryMock repository = new() {
            GetUserByEmailAsyncHandler = (_, _) => Task.FromResult<User?>(null)
        };

        var builder = new LogInServiceBuilder().WithRepository(repository);
        var input = new LogInInputBuilder().Build();

        var result = await builder.Build().ExecuteAsync(input, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.NotFound, result.Error);
    }

    [Fact]
    public async Task Should_Only_Call_GetUserByEmailAsync_And_Only_Once_When_Email_Does_Not_Exist()
    {
        var builder = new LogInServiceBuilder();
        var unitOfWork = builder.UnitOfWork;

        UserRepositoryMock repository = new() {
            GetUserByEmailAsyncHandler = (_, _) => Task.FromResult<User?>(null)
        };

        var service = builder.WithRepository(repository).Build();
        var input = new LogInInputBuilder().Build();

        var result = await service.ExecuteAsync(input, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(1, repository.Tracker.GetCallCount(nameof(repository.GetUserByEmailAsync)));
        Assert.Equal(1, repository.Tracker.TotalCalls);
        Assert.Equal(0, unitOfWork.Tracker.GetCallCount(nameof(unitOfWork.CommitAsync)));
        Assert.Equal(0, unitOfWork.Tracker.TotalCalls);
    }

    [Fact]
    public async Task Should_Return_Failure_When_Unit_Of_Work_Fails()
    {
        var builder = new LogInServiceBuilder();
        var password = "Password123!";
        var user = new UserBuilder().WithPassword(password).WithPasswordHasher(builder.Hasher).Build();

        var repository = new UserRepositoryMock
        {
            GetUserByEmailAsyncHandler = (_, _) => Task.FromResult<User?>(user)
        };

        var unitOfWork = new AuthUnitOfWorkMock()
        {
            CommitAsyncHandler = (_, _) => Task.FromResult(Result.Failure<int>(UnitOfWorkErrors.DatabaseError))
        };

        var service = new LogInServiceBuilder().WithRepository(repository).WithUnitOfWork(unitOfWork).Build();

        var input = new LogInInput(
            Email: user.Email.Value, 
            Password: password, 
            IpAddress: "127.0.0.1", 
            UserAgent: "Mozilla/5.0");

        var result = await service.ExecuteAsync(input, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(UnitOfWorkErrors.DatabaseError, result.Error);
    }

    [Fact]
    public async Task Should_Propagate_Login_Errors()
    {
        var repository = new UserRepositoryMock
        {
            GetUserByEmailAsyncHandler = (_, _) => 
                Task.FromResult<User?>(new UserBuilder().WithPassword("CORRECTpassword123!").Build())
        };
        var service = new LogInServiceBuilder().WithRepository(repository).Build();
        var input = new LogInInputBuilder().WithPassword("WRONGpassword123!").Build();

        var result = await service.ExecuteAsync(input, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.IncorrectPassword, result.Error);
    }

    [Fact]
    public async Task Should_Propagate_Email_Format_Errors_And_Without_Calling_User_Repository()
    {
        var builder = new LogInServiceBuilder();
        var service = builder.Build();
        var input = new LogInInputBuilder().WithEmail("invalid-email").Build();

        var result = await service.ExecuteAsync(input, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.True(result.Error.HasSameCodeId(EmailErrors.Id));
        Assert.Equal(0, builder.Repository.Tracker.TotalCalls);
    }

    [Fact]
    public async Task Should_Propagate_Password_Format_Errors_And_Without_Calling_User_Repository()
    {
        var builder = new LogInServiceBuilder();
        var service = builder.Build();
        var input = new LogInInputBuilder().WithPassword("invalid").Build();

        var result = await service.ExecuteAsync(input, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.True(result.Error.HasSameCodeId(PasswordErrors.Id));
        Assert.Equal(0, builder.Repository.Tracker.TotalCalls);
    }
}