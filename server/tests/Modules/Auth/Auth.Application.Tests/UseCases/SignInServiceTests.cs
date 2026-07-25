using Auth.Application.Tests.Builders;
using Auth.Application.Tests.Mocks;
using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;
using Auth.Application.Tests.Builders.UseCases;

namespace Auth.Application.Tests.UseCases;

public class SignInServiceTests
{
    [Fact]
    public async Task Should_Return_New_User_Guid_When_Input_Is_Valid()
    {
        User? userCreated = null;

        var repository = new UserRepositoryMock
        {
            CreateUserAsyncHandler = (user, _) => 
            { 
                userCreated = user; 
                return Task.CompletedTask; 
            }
        };

        var input = new SignInInputBuilder().Build();
        var service = new SignInServiceBuilder().WithRepository(repository).Build();

        var result = await service.ExecuteAsync(input, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value.UserId);
        Assert.IsType<Guid>(result.Value.UserId);
        Assert.NotNull(userCreated);
        Assert.Equal(userCreated.Id, result.Value.UserId);
    }

    [Fact]
    public async Task Should_Only_Create_User_And_Check_Email_And_Commit_All_Only_Once_When_Input_Is_Valid()
    {
        var input = new SignInInputBuilder().Build();
        var builder = new SignInServiceBuilder();
        var service = builder.Build();
        var repository = builder.Repository;
        var unitOfWork = builder.UnitOfWork;

        var result = await service.ExecuteAsync(input, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value.UserId);
        Assert.Equal(1, repository.Tracker.GetCallCount(nameof(repository.CreateUserAsync)));
        Assert.Equal(1, repository.Tracker.GetCallCount(nameof(repository.IsEmailRegisteredAsync)));
        Assert.Equal(2, repository.Tracker.TotalCalls);
        Assert.Equal(1, unitOfWork.Tracker.GetCallCount(nameof(unitOfWork.CommitAsync)));
        Assert.Equal(1, unitOfWork.Tracker.TotalCalls);
    }

    [Fact]
    public async Task Should_Return_Failure_When_Email_Already_Exists()
    {
        var repository = new UserRepositoryMock
        {
            IsEmailRegisteredAsyncHandler = (_, _) => Task.FromResult(true)
        };

        var input = new SignInInputBuilder().Build();
        var service = new SignInServiceBuilder().WithRepository(repository).Build();

        var result = await service.ExecuteAsync(input, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.EmailAlreadyExists, result.Error);
    }

    [Fact]
    public async Task Should_Return_Failure_When_Unit_Of_Work_Fails()
    {
        var unitOfWork = new AuthUnitOfWorkMock()
        {
            CommitAsyncHandler = (_, _) => Task.FromResult(Result.Failure<int>(UnitOfWorkErrors.DatabaseError))
        };

        var input = new SignInInputBuilder().Build();
        var service = new SignInServiceBuilder().WithUnitOfWork(unitOfWork).Build();

        var result = await service.ExecuteAsync(input, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(UnitOfWorkErrors.DatabaseError, result.Error);
    }

    [Fact]
    public async Task Should_Propagate_User_Creation_Errors()
    {
        var input = new SignInInputBuilder().WithEmail("invalid-email").Build();
        var service = new SignInServiceBuilder().Build();

        var result = await service.ExecuteAsync(input, CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal(EmailErrors.Format, result.Error);
    }

    [Fact]
    public async Task Should_Not_Create_Session_When_Signs_In()
    {
        User? userCreated = null;

        UserRepositoryMock repository = new() {
            CreateUserAsyncHandler = (user, _) => 
            { 
                userCreated = user;
                return Task.CompletedTask; 
            }
        };

        var input = new SignInInputBuilder().Build();
        var service = new SignInServiceBuilder().WithRepository(repository).Build();

        var result = await service.ExecuteAsync(input, CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.NotNull(userCreated);
        Assert.Empty(userCreated.Sessions);
    }
}