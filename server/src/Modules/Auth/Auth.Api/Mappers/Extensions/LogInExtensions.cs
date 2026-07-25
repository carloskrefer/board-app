using Auth.Api.Controllers;
using Auth.Application.UseCases;
using Auth.Domain.Entities;
using Auth.Domain.ValueObjects;
using Core.Api.Builders;
using Core.Api.DTOs.Auth.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Mappers.Extensions;

public static class LogInRequestExtensions
{
    public static LogInInput ToLogInInput(this LogInRequest request, string? ipAddress, string? userAgent) => 
        new (request.Email, request.Password, ipAddress ?? "Unknown", userAgent ?? "Unknown");


    public static LogInResponse ToLogInResponse(this LogInOutput output) => new (output.CredentialsSerialized);

    public static IActionResult ToBadRequest(
        this LogInRequest request, 
        UserController controller, 
        Error error)
    {
        var builder = new BadRequestBuilder(controller);

        if (error == EmailErrors.Format)
            builder.AddError(LogInResponseErrors.EmailFormat(request.Email));

        if (error.HasSameCodeId(PasswordErrors.Id) && !request.Password.Any())
            builder.AddError(LogInResponseErrors.PasswordEmpty(request.Password));

        if (error == UserErrors.NotFound || error == UserErrors.IncorrectPassword)
            builder.AddError(LogInResponseErrors.Credentials);
        
        return builder.Build();
    }

    public static void AddRefreshTokenCookie(this LogInOutput output, IResponseCookies cookies)
    {
        cookies.Append("refreshToken", output.SessionId.ToString(), new CookieOptions
        {
            HttpOnly = true,
            // Secure = true, // TODO: I think this should be enabled in production, test latter whenever HTTPs is working
            SameSite = SameSiteMode.Lax,
            Path = "/api/users",
            Expires = DateTime.UtcNow.AddDays(7) // TODO: Add to configuration file
        });
    }
}