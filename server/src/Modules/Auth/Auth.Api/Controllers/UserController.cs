using Auth.Api.Mappers.Extensions;
using Auth.Application.Commands;
using Core.Api.DTOs.Auth.User;
using Core.Api.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Controllers;


[ApiController]
[Route("/api/users")]
public class UserController : ControllerBase
{
    private readonly IHandler<SignInCommand, Result<SignInOutput>> _signInCommandHandler;
    private readonly IHandler<LogInCommand, Result<LogInOutput>> _logInCommandHandler;
    public UserController(
        IHandler<SignInCommand, Result<SignInOutput>> signInCommandHandler,
        IHandler<LogInCommand, Result<LogInOutput>> logInCommandHandler)
    {
        _signInCommandHandler = signInCommandHandler ?? throw new ArgumentNullException(nameof(SignInCommandHandler));
        _logInCommandHandler = logInCommandHandler ?? throw new ArgumentNullException(nameof(LogInCommandHandler));
    }

    [HttpPost]
    [ProducesResponseType(201, Type=typeof(SignInResponse))]
    [ProducesResponseType<ApiProblemDetails>(400)]
    [Consumes("application/json")]
    [Produces("application/problem+json")]
    public async Task<IActionResult> SignIn([FromBody] SignInRequest request, CancellationToken ct)
    {
        var result = await _signInCommandHandler.HandleAsync(request.ToSignInCommand(), ct);

        if (result.IsFailure)
            return request.ToBadRequest(this, result.Error);

        var responseBody = result.Value.ToSignInResponse();

        return Created($"/users/{result.Value.UserId}", responseBody);
    }

    [HttpPost("login")]
    [ProducesResponseType(200, Type=typeof(LogInResponse))]
    [ProducesResponseType<ApiProblemDetails>(400)]
    [Consumes("application/json")]
    [Produces("application/problem+json")]
    public async Task<IActionResult> LogIn([FromBody] LogInRequest request, CancellationToken ct)
    {
        var command = request.ToLogInCommand(
            ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(), 
            userAgent: HttpContext.Request.Headers["User-Agent"].ToString());

        var result = await _logInCommandHandler.HandleAsync(command, ct);

        if (result.IsFailure)
            return request.ToBadRequest(this, result.Error);

        result.Value.AddRefreshTokenCookie(HttpContext.Response.Cookies);

        return Ok(result.Value.ToLogInResponse());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(CancellationToken ct)
    {
        // Implementation for getting a user by ID
        return Ok();
    }
}