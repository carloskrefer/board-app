using Core.Api.Errors;
using Core.String.Extensions;

namespace Core.Api.DTOs.Auth.User;

public record LogInRequest(string Email, string Password);

public record LogInResponse(string AccessToken);

public static class LogInResponseErrors
{
    public static GeneralResponseError Credentials =>
        new(
            Code: "CREDENTIALS",
            Message: "Email is not registered or password is incorrect."
        );

    public static DetailedResponseError EmailFormat(string rejectedValue) =>
        new(
            EmailResponseErrors.Format,
            nameof(LogInRequest.Email).FromPascalCaseToCamelCase(),
            ErrorLocationEnum.Body,
            rejectedValue
        );

    public static DetailedResponseError PasswordEmpty(string rejectedValue) =>
        new(
            "PASSWORD.EMPTY",
            "The provided password should not be empty.",
            nameof(LogInRequest.Password).FromPascalCaseToCamelCase(),
            ErrorLocationEnum.Body,
            rejectedValue
        );
}