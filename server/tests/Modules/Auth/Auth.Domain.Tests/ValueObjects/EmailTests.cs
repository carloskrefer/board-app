using Auth.Domain.ValueObjects;

namespace Auth.Domain.Tests.ValueObjects;

public class EmailTests
{
    [Theory]
    [InlineData("a@a")]
    [InlineData("a@a.a")]
    [InlineData("a.a@a.a.a")]
    [InlineData("a@example.com")]
    [InlineData("a@example.com.br")]
    [InlineData("test@example.com")]
    [InlineData("John@example.com")]
    [InlineData("John-Doe@example.com")]
    [InlineData("john_doe@example.com")]
    [InlineData("john123@example.com")]
    public void Should_Create_Email_When_Is_Valid(string email)
    {
        var result = Email.Create(email);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
    }

    [Theory]
    [InlineData("John-Doe_123@Example.Com")]
    [InlineData(" John-Doe_123@Example.Com  ")]
    public void Should_Normalize_Email_When_Valid(string email)
    {
        var result = Email.Create(email);

        Assert.True(result.IsSuccess);
        Assert.Equal("john-doe_123@example.com", result.Value.Value);
    }

    [Theory]
    [InlineData("invalid-email")]
    [InlineData("invalid@")]
    [InlineData("@invalid.com")]
    [InlineData("@@@")]
    [InlineData("a@a.com@a@a")]
    public void Should_Not_Create_Email_When_Invalid(string email)
    {
        var result = Email.Create(email);

        Assert.False(result.IsSuccess);
    }
}