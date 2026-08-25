using Constracts.Serializing;
using Contracts.Errors;

namespace Contracts.Auth;

public record LogInRequest(string Email, string Password);
public record LogInResponse(string AccessToken);

public static class LogInResponseErrors
{
    public static BaseError Credentials =>
        new(
            Code: "CREDENTIALS",
            Message: "Email is not registered or password is incorrect."
        );

    public static DetailedError EmailFormat(string rejectedValue) =>
        new(
            EmailResponseErrors.Format,
            nameof(LogInRequest.Email).ToCamelCase(),
            ErrorLocationEnum.Body,
            rejectedValue
        );

    public static DetailedError PasswordEmpty(string rejectedValue) =>
        new(
            "PASSWORD.EMPTY",
            "The provided password should not be empty.",
            nameof(LogInRequest.Password).ToCamelCase(),
            ErrorLocationEnum.Body,
            rejectedValue
        );
}