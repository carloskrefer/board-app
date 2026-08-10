using Core.Api.Errors;
using Core.String.Extensions;

namespace Core.Api.DTOs.Auth.User;

public record SignInRequest(string Email, string Name, string Password);
public record SignInResponse(Guid UserId);

public static class SignInResponseErrors
{
    public static DetailedResponseError EmailFormat(string rejectedValue) =>
        new(
            EmailResponseErrors.Format,
            nameof(SignInRequest.Email).FromPascalCaseToCamelCase(),
            ErrorLocationEnum.Body,
            rejectedValue
        );

    public static DetailedResponseError NameEmpty(string rejectedValue) =>
        new(
            TextResponseErrors.Empty("Name"),
            nameof(SignInRequest.Name).FromPascalCaseToCamelCase(),
            ErrorLocationEnum.Body,
            rejectedValue
        );

    public static DetailedResponseError NameTooLong(string rejectedValue) =>
        new(
            TextResponseErrors.TooLong(100, "Name"),
            nameof(SignInRequest.Name).FromPascalCaseToCamelCase(),
            ErrorLocationEnum.Body,
            rejectedValue
        );

    public static DetailedResponseError NameOnlySpecialCharacters(string rejectedValue) =>
        new(
            TextResponseErrors.OnlySpecialCharacters("Name"),
            nameof(SignInRequest.Name).FromPascalCaseToCamelCase(),
            ErrorLocationEnum.Body,
            rejectedValue
        );

    public static DetailedResponseError PasswordLength(string rejectedValue) =>
        new(
            TextResponseErrors.BetweenRange(6, 100, "Password"),
            nameof(SignInRequest.Password).FromPascalCaseToCamelCase(),
            ErrorLocationEnum.Body,
            rejectedValue
        );

    public static DetailedResponseError PasswordSpecialCharacters(string rejectedValue) =>
        new(
            "SPECIAL_CHARACTERS", 
            "Password must contain at least one special character. Examples: !?@#$%^&*()-+.",
            nameof(SignInRequest.Password).FromPascalCaseToCamelCase(),
            ErrorLocationEnum.Body,
            rejectedValue
        );

    public static DetailedResponseError PasswordUpperAndLowerCase(string rejectedValue) =>
        new(
            "UPPER_AND_LOWER_CASE", 
            "Password must contain at least one uppercase and one lowercase letter.",
            nameof(SignInRequest.Password).FromPascalCaseToCamelCase(),
            ErrorLocationEnum.Body,
            rejectedValue
        );

    public static DetailedResponseError PasswordNumber(string rejectedValue) =>
        new(
            "NUMBERS", 
            "Password must contain at least one number.",
            nameof(SignInRequest.Password).FromPascalCaseToCamelCase(),
            ErrorLocationEnum.Body,
            rejectedValue
        );  
}