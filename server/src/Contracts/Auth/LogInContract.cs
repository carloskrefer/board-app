using System.ComponentModel.DataAnnotations;
using Contracts.Errors;

namespace Contracts.Auth;

public record LogInRequest
{
    /// <summary>
    /// User email.
    /// </summary>
    /// <example>john@example.com</example>
    [Required]
    [EmailAddress]
    public string? Email { get; init; }

    /// <summary>
    /// User password.
    /// </summary>  
    /// <example>Password123!</example>
    [Required]
    [MinLength(6)]
    [MaxLength(100)]  
    public string? Password { get; init; }

    public LogInRequest(string? email, string? password)
    {
        Email = email;
        Password = password;
    }
}
public record LogInResponse
{
    /// <summary>
    /// JWT token used for stateless authentication.
    /// </summary>
    /// <example>john@example.com</example>
    [Required]
    public string AccessToken { get; set; }

    public LogInResponse(string accessToken)
    {
        AccessToken = accessToken;
    }
}

public static class LogInResponseErrors
{
    public static DetailedApiError Credentials =>
        new(
            code: "LOGIN_CREDENTIALS",
            message: "Email is not registered or password is incorrect.",
            field: null,
            location: ErrorLocation.Body,
            rejectedValue: null);
}