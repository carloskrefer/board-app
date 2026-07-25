using Auth.Domain.Entities;
using Auth.Domain.Tests.Builders;
using Auth.Domain.Tests.Mocks;
using Auth.Domain.ValueObjects;

namespace Auth.Domain.Tests.Entities;

public class UserTests
{
    [Fact]
    public void Create_Should_Create_User_When_All_Inputs_Are_Valid()
    {
        const string email = "joshua@example.com";
        const string password = "Senha@123";
        const string name = "Joshua";

        var before = DateTime.UtcNow;

        var result = User.Create(email, password, name, new PasswordHasherMock());

        var after = DateTime.UtcNow;

        Assert.True(result.IsSuccess);

        var user = result.Value;

        Assert.NotEqual(Guid.Empty, user.Id);
        Assert.Equal(email, user.Email.Value);
        Assert.Equal(name, user.Name.Value);

        Assert.False(string.IsNullOrWhiteSpace(user.PasswordHash));

        Assert.True(user.CreatedAt >= before);
        Assert.True(user.CreatedAt <= after);

        Assert.Null(user.UpdatedAt);
    }

    [Fact]
    public void Create_Should_Start_With_Zero_Sessions_When_Inputs_Are_Valid()
    {
        var result = User.Create("joshua@example.com", "Senha@123", "Joshua", new PasswordHasherMock());

        Assert.True(result.IsSuccess);
        Assert.Empty(result.Value.Sessions);
    }

    [Theory]
    [InlineData("a", "Senha@123", "Joshua")]
    [InlineData("joshua@example.com", "123", "Joshua")]
    [InlineData("joshua@example.com", "Senha@123", " ")]
    public void Create_Should_Fail_When_Inputs_Are_Invalid(string email, string password, string name)
    {
        var result = User.Create(email, password, name, new PasswordHasherMock());

        Assert.False(result.IsSuccess);
    }

    [Fact]
    public void Create_Should_Propagate_Email_Errors_When_Email_Is_Invalid()
    {
        const string emailInvalid = "invalid-email";
        const string password = "Senha@123";
        const string name = "Joshua";

        var result = User.Create(emailInvalid, password, name, new PasswordHasherMock());

        Assert.False(result.IsSuccess);
        Assert.Equal(EmailErrors.Format, result.Error);
    }

    [Fact]
    public void Create_Should_Propagate_Name_Errors_When_Name_Is_Invalid()
    {
        const string email = "joshua@example.com";
        const string password = "Senha@123";
        const string nameInvalid = " ";

        var result = User.Create(email, password, nameInvalid, new PasswordHasherMock());

        Assert.False(result.IsSuccess);
        Assert.Equal(NameErrors.Empty, result.Error);
    }

    [Fact]
    public void Create_Should_Propagate_Password_Errors_When_Password_Is_Invalid()
    {
        const string email = "joshua@example.com";
        const string passwordInvalid = "1";
        const string name = "Joshua";

        var result = User.Create(email, passwordInvalid, name, new PasswordHasherMock());

        Assert.False(result.IsSuccess);
        Assert.Equal(PasswordErrors.Length, result.Error);
    }

    [Fact]
    public void Create_Should_Hash_Password_When_Input_Is_Valid()
    {
        const string email = "joshua@example.com";
        const string password = "Senha@123";
        const string name = "Joshua";

        var hasher = new PasswordHasherMock();

        var result = User.Create(email, password, name, hasher);

        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Value.PasswordHash);
        Assert.NotEqual(password, result.Value.PasswordHash);
        Assert.Equal(hasher.Hash(password), result.Value.PasswordHash);
    }

    [Fact]
    public void LogIn_Should_Return_Correct_Session_When_Credentials_Are_Valid()
    {
        const string ipAddress = "192.168.100.1";
        const string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) " +
            "Chrome/58.0.3029.110 Safari/537.3";

        var builder = new UserBuilder();
        var user = builder.Build();

        var result = user.LogIn(builder._password, builder._passwordHasher, ipAddress, userAgent);

        Assert.True(result.IsSuccess);
        Assert.Equal(user.Id, result.Value.UserId);
        Assert.Equal(ipAddress, result.Value.IpAddress);
        Assert.Equal(userAgent, result.Value.UserAgent);
    }

    [Fact]
    public void Login_Should_Fail_And_Return_IncorrectPassword_Error_And_Not_Create_Session_When_Credentials_Are_Invalid()
    {
        var builder = new UserBuilder();
        var user = builder.WithPassword("CORRECTpassword123!").Build();

        var result = user.LogIn("WRONGpassword123!", builder._passwordHasher, "192.168.100.1", "Mozilla");

        Assert.False(result.IsSuccess);
        Assert.Equal(UserErrors.IncorrectPassword, result.Error);
        Assert.Empty(user.Sessions);
    }

    [Fact]
    public void Login_Should_Not_Remove_Or_Change_Existing_Sessions_When_Credentials_Are_Invalid()
    {
        var ipAddress = "192.168.100.1";
        const string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) " +
            "Chrome/58.0.3029.110 Safari/537.3";

        var builder = new UserBuilder();
        var user = builder.Build();

        var session = user.LogIn(builder._password, builder._passwordHasher, ipAddress, userAgent).Value;

        var result = user.LogIn("wrongpassword", builder._passwordHasher, "192.168.100.2", "Safari");

        Assert.False(result.IsSuccess);
        Assert.Single(user.Sessions);
        Assert.Equal(session.Id, user.Sessions.First().Id);
        Assert.Equal(session, user.Sessions.First());
        Assert.Equal(ipAddress, user.Sessions.First().IpAddress);
        Assert.Equal(userAgent, user.Sessions.First().UserAgent);
    }
}