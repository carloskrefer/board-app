using Auth.Api.Mappers.Extensions;
using Auth.Api.Utils;
using Auth.Api.Validators;
using Auth.Application.Commands;
using Contracts.Auth;
using Core.Api;
using Core.Api.Authentication;
using Core.Api.OpenApi;
using Core.Api.OpenApi.Attributes;
using Core.Api.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Controllers;

public static class Constants
{
    public const string Route = "/api/users";
}


[ApiController]
[Tags(Tags.Auth)]
[Route(Constants.Route)]
public class UserController(
    IHandler<SignInCommand, Result<SignInOutput>> signInCommandHandler,
    IHandler<LogInCommand, Result<LogInOutput>> logInCommandHandler,
    IHandler<LogOutCommand, Result<LogOutOutput>> logOutCommandHandler,
    ProblemResultFactory2 problemResultFactory) : ControllerBase
{
    // TODO: rewrite tests considering the null values and stuff.
    // TODO: Ver documento de Bearer e conferir se retorna WWW-Authenticate certinho (https://docs.google.com/document/d/1gWZDYVYuD-INXb6p__qleR9w7NBCn-36no9V7lITh9Y/edit?tab=t.0)
    // TODO: Rename app tests to Command prefixes, removing "use case" from name or namespace
    // TODO: Find a prettier way to add decorators

    /// <summary>
    /// Registers a new user.
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType<SignInResponse>(StatusCodes.Status201Created)]
    public async Task<IActionResult> SignIn([FromBody] SignInRequest? request, CancellationToken ct = default)
    {
        if (!request.Validate(out var errors))
            return problemResultFactory.CreateBadRequest(errors);

        var result = await signInCommandHandler.HandleAsync(request.ToSignInCommand(), ct);

        if (result.IsFailure)
            return result.Error.ToResponse(request, problemResultFactory);

        var responseBody = result.Value.ToSignInResponse();

        return Created($"/users/{result.Value.UserId}", responseBody);
    }

    // TODO: Continuar checando essa questão da documentação do swagger
    // TODO: Documentar no OpenAPI o header de refresh token via cookie. No endpoint de loguot também.

    /// <summary>
    /// Logs user in with the provided credentials.
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [SetCookie(
        whenStatusCode: StatusCodes.Status200OK,
        key: "refreshToken",
        value: "<sessionId>",
        description: "Refresh token for maintaining user session.",
        isHttpOnly: true,
        isSecure: true,
        sameSiteMode: Microsoft.Net.Http.Headers.SameSiteMode.Lax,
        daysExpiration: 7,
        path: "/api/users",
        required: true
    )]
    [ProducesResponseType<LogInResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> LogIn([FromBody] LogInRequest? request, CancellationToken ct = default)
    {
        if (!request.Validate(out var errors))
            return problemResultFactory.CreateBadRequest(errors);

        var command = request.ToLogInCommand(
            ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(),
            userAgent: HttpContext.Request.Headers["User-Agent"].ToString());

        var result = await logInCommandHandler.HandleAsync(command, ct);

        if (result.IsFailure)
            return result.Error.ToResponse(request, problemResultFactory);

        HttpContext.Response.Cookies.AddRefreshTokenCookie(result.Value.SessionId, Constants.Route);

        return Ok(result.Value.ToLogInResponse());
    }

    // TODO: Check if there is a parameter for the cookie. Even if not, this needs to be documented for OpenAPI.    
    // TODO: Add tests to logout
    // TODO: Check if it actually removes the cookie

    /// <summary>
    /// Logs user out and invalidates the current session.
    /// </summary>
    [HttpPost("logout")]
    [SetCookie(
        whenStatusCode: StatusCodes.Status204NoContent,
        key: "refreshToken", // TODO: Store this constant somewhere, it repeated a bunch of times already.
        value: "<sessionId>",
        description: "Removes the Refresh Token Cookie as user is logging out.",
        isHttpOnly: true,
        isSecure: true,
        sameSiteMode: Microsoft.Net.Http.Headers.SameSiteMode.Lax,
        maxAge: 0,
        path: "/api/users",
        required: true
    )]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> LogOut(CancellationToken ct = default)
    {
        var command = new LogOutCommand(
            UserId: User.GetSub(),
            SessionId: Request.Cookies.GetRefreshToken());

        var result = await logOutCommandHandler.HandleAsync(command, ct);

        if (result.IsFailure)
            return command.ToErrorResponse(HttpContext, ProblemDetailsFactory, result.Error);

        HttpContext.Response.Cookies.DeleteRefreshTokenCookie(Constants.Route);

        return NoContent();
    }
}