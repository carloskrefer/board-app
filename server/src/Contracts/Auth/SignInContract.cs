using Contracts.Errors;
using Constracts.Serializing;
using System.ComponentModel.DataAnnotations;

namespace Contracts.Auth;

public record SignInRequest
{
    /// <summary>
    /// User email.
    /// </summary>  
    /// <example>john@example.com</example>
    [Required]
    [EmailAddress]
    public string? Email { get; set; }

    /// <summary>
    /// User full name.
    /// </summary>  
    /// <example>John Doe</example>
    [Required]
    [MinLength(1)]
    [MaxLength(100)]
    public string? Name { get; set; }

    /// <summary>
    /// User password.
    /// </summary>  
    /// <example>Password123!</example>
    [Required]
    [MinLength(6)]
    [MaxLength(100)]
    public string? Password { get; set; }
}

public record SignInResponse
{
    /// <summary>
    /// User ID.
    /// </summary>  
    /// <example>a80d2be5-f9f0-49ac-9f91-ed5a149b4cd9</example>
    [Required]
    public Guid UserId { get; init; }
}


public static class SignInResponseErrors
{
    public static DetailedApiError EmailAlreadyRegistered(string rejectedValue) =>
        new(
            EmailResponseErrors.AlreadyRegistered,
            nameof(SignInRequest.Email).ToCamelCase(),
            ErrorLocation.Body,
            rejectedValue);
}