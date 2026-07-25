using Core.String.Extensions;

namespace Auth.Domain.ValueObjects;

public sealed record Password(string Value)
{
    public static Result<Password> Create(string value)
    {
        if (value.Length < 6 || value.Length > 100)
            return PasswordErrors.Length;

        if (!value.HasAtLeastOneSpecialCharacter())
            return PasswordErrors.SpecialCharacters;

        if (!value.HasAtLeastOneUpperAndLowerCaseLetter())
            return PasswordErrors.UpperAndLowerCase;

        if (!value.HasAtLeastOneNumber())
            return PasswordErrors.Number;

        return new Password(value);
    }
}

public static class PasswordErrors
{
    public static readonly string Id = "PASSWORD";
    public static readonly Error Length = 
        new(
            $"{Id}.LENGTH", 
            "Password must be between 6 and 100 characters.");

    public static readonly Error SpecialCharacters = 
        new(
            $"{Id}.SPECIAL_CHARACTERS", 
            "Password must contain at least one special character. Examples: !?@#$%^&*()-+.");

    public static readonly Error UpperAndLowerCase = 
        new(
            $"{Id}.UPPER_AND_LOWER_CASE", 
            "Password must contain at least one uppercase and one lowercase letter.");

    public static readonly Error Number = 
        new(
            $"{Id}.NUMBER", 
            "Password must contain at least one number.");
}