using Auth.Domain.ValueObjects;

namespace Auth.Domain.Tests.ValueObjects;

public class NameTests
{
    [Theory]
    [InlineData("J")]
    [InlineData("JD")]
    [InlineData("John")]
    [InlineData("John-Doe")]
    [InlineData("John-Doe Smith")]
    [InlineData("John-Doe Smith Jr.")]
    public void Should_Create_Name_When_Is_Valid(string name)
    {
        var result = Name.Create(name);

        Assert.True(result.IsSuccess);

        var createdName = result.Value;

        Assert.Equal(name, createdName.Value);
    }

    [Theory]
    [InlineData(" John ")]
    [InlineData("   John  ")]
    [InlineData("   John Doe  ")]
    public void Should_Normalize_Name_When_Is_Untrimmed(string name)
    {
        var result = Name.Create(name);

        Assert.True(result.IsSuccess);

        var createdName = result.Value;

        Assert.Equal(name.Trim(), createdName.Value);
    }

    [Theory]
    [InlineData("John        Doe Jack")]
    [InlineData("   John        Doe   Jack")]
    [InlineData("   John        Doe   Jack  ")]
    public void Should_Normalize_Name_When_Has_Extra_Spaces(string name)
    {
        var result = Name.Create(name);

        Assert.True(result.IsSuccess);

        var createdName = result.Value;

        Assert.Equal("John Doe Jack", createdName.Value);
    }

    [Fact]
    public void Should_Not_Create_Name_When_Name_Is_Long()
    {
        string longName = new string('A', 101);

        var result = Name.Create(longName);

        Assert.False(result.IsSuccess);
        Assert.Equal(NameErrors.TooLong, result.Error);
    }

    [Fact]
    public void Should_Create_Name_When_Name_Has_Maximum_Length()
    {
        string longName = new string('A', 100);

        var result = Name.Create(longName);

        Assert.True(result.IsSuccess);
        Assert.Equal(longName, result.Value.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public void Should_Not_Create_Name_When_Name_Is_Empty(string name)
    {
        var result = Name.Create(name);

        Assert.False(result.IsSuccess);
        Assert.Equal(NameErrors.Empty, result.Error);
    }

    [Theory]
    [InlineData("!")]
    [InlineData(".")]
    [InlineData("!!")]
    [InlineData("!!@ #  ")]
    public void Should_Not_Create_Name_When_Name_Only_Has_Special_Characters(string name)
    {
        var result = Name.Create(name);

        Assert.False(result.IsSuccess);
        Assert.Equal(NameErrors.OnlySpecialCharacters, result.Error);
    }
}