using Auth.Domain.ValueObjects;

namespace Auth.Domain.Tests.ValueObjects;

public class PasswordTests
{
    [Theory]
    [InlineData("AAaa1!")]
    [InlineData("aA1234@")]
    public void Should_Create_Password_When_Password_Is_Valid(string password)
    {
        var result = Password.Create(password);

        Assert.True(result.IsSuccess);

        var createdPassword = result.Value;

        Assert.Equal(password, createdPassword.Value);
    }

    [Fact]
    public void Should_Not_Create_Password_When_Password_Has_More_Than_100_Characters()
    {
        var result = Password.Create($"aA1!{new string('A', 101)}");

        Assert.False(result.IsSuccess);
        Assert.Equal(PasswordErrors.Length, result.Error);
    }

    [Fact]
    public void Should_Not_Create_Password_When_Password_Has_No_Special_Characters()
    {
        var result = Password.Create("Password123");

        Assert.False(result.IsSuccess);
        Assert.Equal(PasswordErrors.SpecialCharacters, result.Error);
    }

    [Theory]
    [InlineData("!@#!@#!@#")]
    [InlineData("password123!")]
    [InlineData("PASSWORD123!")]
    public void Should_Not_Create_Password_When_Password_Does_Not_Include_Both_Upper_And_Lower_Case_Letters(string password)
    {
        var result = Password.Create(password);

        Assert.False(result.IsSuccess);
        Assert.Equal(PasswordErrors.UpperAndLowerCase, result.Error);
    }

    [Fact]
    public void Should_Not_Create_Password_When_Password_Has_No_Numbers()
    {
        var result = Password.Create("Password!");

        Assert.False(result.IsSuccess);
        Assert.Equal(PasswordErrors.Number, result.Error);
    }
}