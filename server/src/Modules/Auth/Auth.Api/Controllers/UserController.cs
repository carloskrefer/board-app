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
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Controllers;

[ApiController]
[Tags(Tags.Auth)]
[Route("/api/users")]
public class UserController(
    IHandler<SignInCommand, Result<SignInOutput>> signInCommandHandler,
    IHandler<LogInCommand, Result<LogInOutput>> logInCommandHandler,
    IHandler<LogOutCommand, Result<LogOutOutput>> logOutCommandHandler,
    ProblemResultFactory2 problemResultFactory) : ControllerBase
{
    // TODO: Fazer os outros serem "?" também os outros requests.
    // TODO: Anotar num architectural decision sobre essa escolha de desativar a validação automática do model state,
    // pq o sign in request é tudo "?" os objetos...
    // TODO: rewrite tests considering the null values and stuff.

    /// <summary>
    /// Registers a new user.
    /// </summary>
    [HttpPost]
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

        HttpContext.Response.Cookies.AddRefreshTokenCookie(result.Value.SessionId);

        return Ok(result.Value.ToLogInResponse());
    }

    // TODO: Check if there is a parameter for the cookie. Even if not, this needs to be documented for OpenAPI.    
    // TODO: Add tests to logout

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

        HttpContext.Response.Cookies.DeleteRefreshTokenCookie();

        return NoContent();
    }
}