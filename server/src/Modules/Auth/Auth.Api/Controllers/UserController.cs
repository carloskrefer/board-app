using Auth.Api.Mappers.Extensions;
using Auth.Application.UseCases;
using Core.Api.DTOs.Auth.User;
using Core.Api.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Auth.Api.Controllers;


[ApiController]
[Route("/api/users")]
public class UserController : ControllerBase
{
    private readonly IUseCase<SignInInput, Result<SignInOutput>> _signInService;
    private readonly IUseCase<LogInInput, Result<LogInOutput>> _logInService;
    public UserController(
        IUseCase<SignInInput, Result<SignInOutput>> signInService,
        IUseCase<LogInInput, Result<LogInOutput>> logInService)
    {
        _signInService = signInService ?? throw new ArgumentNullException(nameof(signInService));
        _logInService = logInService ?? throw new ArgumentNullException(nameof(logInService));
    }

    [HttpPost]
    [ProducesResponseType(201, Type=typeof(SignInResponse))]
    [ProducesResponseType<ApiProblemDetails>(400)]
    [Consumes("application/json")]
    [Produces("application/problem+json")]
    public async Task<IActionResult> SignIn([FromBody] SignInRequest request, CancellationToken ct)
    {
        var result = await _signInService.ExecuteAsync(request.ToSignInInput(), ct);

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
        var input = request.ToLogInInput(
            ipAddress: HttpContext.Connection.RemoteIpAddress?.ToString(), 
            userAgent: HttpContext.Request.Headers["User-Agent"].ToString());

        var result = await _logInService.ExecuteAsync(input, ct);

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