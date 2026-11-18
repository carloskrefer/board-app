using System.Net.Http.Json;
using Auth.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestsCommon.Persistance.Factories;
using TestsCommon.Persistance.Base;
using Auth.Api.Tests.CollectionFixtures;
using Contracts.Auth;
using Auth.Api.Tests.Builders;
using TestsCommon.Extensions;
using TestsCommon.Helpers;
using TestsCommon.Assertions;
using Contracts.Errors;
using Core.String.Extensions;
using Auth.Api.Tests.Mocks;
using Auth.Application.Interfaces;

namespace Auth.Api.Tests.IntegrationTests.UserControllerTests;

[Collection(CollectionFixturesNames.DefaultIntegrationTestsCollection)]
public class LogInTests : DefaultIntegrationTests<AuthDbContext>
{
    private const string _signInUrl = "/api/users";
    private const string _loginUrl = "/api/users/login";
    
    public LogInTests(DefaultTestingWebApplicationFactory<AuthDbContext> factory) : base(factory) { }

    [Fact]
    public async Task Should_Create_And_Return_Session_For_Correct_User()
    {
        var email = "test@example.com";
        var password = "ValidPassword123!";
        var signInBody = new SignInRequest{
            Email = email, 
            Name = "Joshua", 
            Password = password
        };
        await _client.PostAsJsonAsync(_signInUrl, signInBody);

        var body = new LogInRequest(email, password);
        var response = await _client.PostAsJsonAsync(_loginUrl, body);
        
        Assert.Equal(System.Net.HttpStatusCode.OK, response.StatusCode);
        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        var user = await db.Users.Include(u => u.Sessions).SingleAsync(u => u.Email.Value.Equals(email));
        Assert.Single(user.Sessions);
    }

    [Fact]
    public async Task Should_Return_Correct_Refresh_Token_Cookie()
    {
        var signInRequest = new SignInRequestBuilder().Build();
        await _client.PostAsJsonAsync(_signInUrl, signInRequest);

        var body = new LogInRequest(signInRequest.Email, signInRequest.Password);
        var response = await _client.PostAsJsonAsync(_loginUrl, body);

        response.Headers.SingleCookieByName("refreshToken", out var cookieValue, out var cookieAttributes); 
        var expiration = CookieHelper.SingleExpirationUtcFromCookieAttributes(cookieAttributes);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        
        var session = db.Users
            .Include(u => u.Sessions)
            .Single(u => u.Email.Value.Equals(signInRequest.Email))
            .Sessions
            .Single();
        
        Assert.Equal(session.Id, Guid.Parse(cookieValue));
        Assert.Contains("HttpOnly", cookieAttributes, StringComparer.OrdinalIgnoreCase);
        Assert.Contains("SameSite=Lax", cookieAttributes, StringComparer.OrdinalIgnoreCase);
        Assert.Contains("Path=/api/users", cookieAttributes, StringComparer.OrdinalIgnoreCase);
        // Assert.Contains("Secure", cookieAttributes, StringComparer.OrdinalIgnoreCase); // TODO: Enable in the future.
        Assert.Equal(DateTime.UtcNow.AddDays(7).Date, expiration.Date);
    }

    [Fact]
    public async Task Should_Respond_With_BadRequest_And_Generic_Credentials_Error_When_User_Does_Not_Exist()
    {
        var body = new LogInRequest("nonexistent@example.com", "anyPassword123!");
        var response = await _client.PostAsJsonAsync(_loginUrl, body);

        await BadRequestAssertions.AssertResponse(response, _loginUrl, [LogInResponseErrors.Credentials]);
    }

    [Fact]
    public async Task Should_Respond_With_BadRequest_And_Generic_Credentials_Error_When_Password_Is_Incorrect()
    {
        var signInRequest = new SignInRequestBuilder().Build();
        await _client.PostAsJsonAsync(_signInUrl, signInRequest);

        var body = new LogInRequest(signInRequest.Email, "IncorrectPassword123!");
        var response = await _client.PostAsJsonAsync(_loginUrl, body);

        await BadRequestAssertions.AssertResponse(response, _loginUrl, [LogInResponseErrors.Credentials]);
    }

    [Fact]
    public async Task Should_Respond_With_BadRequest_And_Generic_Credentials_Error_When_Password_Format_Is_Invalid()
    {
        var signInRequest = new SignInRequestBuilder().Build();
        await _client.PostAsJsonAsync(_signInUrl, signInRequest);

        var body = new LogInRequest(signInRequest.Email, "invalidformat");
        var response = await _client.PostAsJsonAsync(_loginUrl, body);

        await BadRequestAssertions.AssertResponse(response, _loginUrl, [LogInResponseErrors.Credentials]);
    }

    [Fact]
    public async Task Should_Respond_With_BadRequest_When_Email_Format_Is_Invalid()
    {
        var body = new LogInRequest("invalid-email", "AnyPassword123!");
        var response = await _client.PostAsJsonAsync(_loginUrl, body);

        List<DetailedApiError> expectedErrors = [
            new(
                generalError: EmailResponseErrors.Format,
                field: nameof(body.Email).FromPascalCaseToCamelCase(),
                location: ErrorLocation.Body,
                rejectedValue: body.Email)
        ];

        await BadRequestAssertions.AssertResponse(response, _loginUrl, expectedErrors);
    }

    [Fact]
    public async Task Should_Respond_With_BadRequest_When_Password_Is_Empty()
    {
        var body = new LogInRequest("test@example.com", "");
        var response = await _client.PostAsJsonAsync(_loginUrl, body);

        List<DetailedApiError> expectedErrors = [
            new(
                generalError: TextResponseErrors.Empty("Password"),
                field: nameof(body.Password).FromPascalCaseToCamelCase(),
                location: ErrorLocation.Body)
        ];

        await BadRequestAssertions.AssertResponse(response, _loginUrl, expectedErrors);
    }

    [Fact]
    public async Task Should_Respond_With_409_Conflict_When_Concurrency_Error_Happens()
    {
        var signInRequest = new SignInRequestBuilder().Build();
        var mockResult = Result.Failure<int>(OptimisticUnitOfWorkErrors.UpdateConcurrency);

        var unitOfWorkMock = new AuthUnitOfWorkMock()
        {
            CommitAsyncHandler = (_, _) => Task.FromResult(mockResult)
        };

        using var customFactory = _factory.WithWebHostBuilder(whb =>
            whb.ConfigureServices(sc => sc.AddScoped<IAuthUnitOfWork>(sp => unitOfWorkMock)));

        using var customClient = customFactory.CreateClient();
        var body = new LogInRequest(signInRequest.Email, signInRequest.Password);

        await _client.PostAsJsonAsync(_signInUrl, signInRequest);
        var response = await customClient.PostAsJsonAsync(_loginUrl, body);

        Assert.Equal(System.Net.HttpStatusCode.Conflict, response.StatusCode);
    }    
}