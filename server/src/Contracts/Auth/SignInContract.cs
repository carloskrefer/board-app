using Contracts.Errors;
using Constracts.Serializing;

namespace Contracts.Auth;

public record SignInRequest(string Email, string Name, string Password);
public record SignInResponse(Guid UserId);

public static class SignInResponseErrors
{
    public static DetailedError EmailAlreadyRegistered(string rejectedValue) =>
        new(
            EmailResponseErrors.AlreadyRegistered,
            nameof(SignInRequest.Email).ToCamelCase(),
            ErrorLocationEnum.Body,
            rejectedValue
        );

    public static DetailedError EmailFormat(string rejectedValue) =>
        new(
            EmailResponseErrors.Format,
            nameof(SignInRequest.Email).ToCamelCase(),
            ErrorLocationEnum.Body,
            rejectedValue
        );

    public static DetailedError NameEmpty(string rejectedValue) =>
        new(
            TextResponseErrors.Empty("Name"),
            nameof(SignInRequest.Name).ToCamelCase(),
            ErrorLocationEnum.Body,
            rejectedValue
        );

    public static DetailedError NameTooLong(string rejectedValue) =>
        new(
            TextResponseErrors.TooLong(100, "Name"),
            nameof(SignInRequest.Name).ToCamelCase(),
            ErrorLocationEnum.Body,
            rejectedValue
        );

    public static DetailedError NameOnlySpecialCharacters(string rejectedValue) =>
        new(
            TextResponseErrors.OnlySpecialCharacters("Name"),
            nameof(SignInRequest.Name).ToCamelCase(),
            ErrorLocationEnum.Body,
            rejectedValue
        );

    public static DetailedError PasswordLength(string rejectedValue) =>
        new(
            TextResponseErrors.BetweenRange(6, 100, "Password"),
            nameof(SignInRequest.Password).ToCamelCase(),
            ErrorLocationEnum.Body,
            rejectedValue
        );

    public static DetailedError PasswordSpecialCharacters(string rejectedValue) =>
        new(
            "SPECIAL_CHARACTERS", 
            "Password must contain at least one special character. Examples: !?@#$%^&*()-+.",
            nameof(SignInRequest.Password).ToCamelCase(),
            ErrorLocationEnum.Body,
            rejectedValue
        );

    public static DetailedError PasswordUpperAndLowerCase(string rejectedValue) =>
        new(
            "UPPER_AND_LOWER_CASE", 
            "Password must contain at least one uppercase and one lowercase letter.",
            nameof(SignInRequest.Password).ToCamelCase(),
            ErrorLocationEnum.Body,
            rejectedValue
        );

    public static DetailedError PasswordNumber(string rejectedValue) =>
        new(
            "NUMBERS", 
            "Password must contain at least one number.",
            nameof(SignInRequest.Password).ToCamelCase(),
            ErrorLocationEnum.Body,
            rejectedValue
        );  
}