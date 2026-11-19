using System.Net.Http.Json;
using Auth.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestsCommon.Persistance.Factories;
using TestsCommon.Persistance.Base;
using Auth.Api.Tests.CollectionFixtures;
using Contracts.Errors;
using Core.String.Extensions;
using TestsCommon.Assertions;
using Contracts.Auth;
using System.Net;

namespace Auth.Api.Tests.IntegrationTests.UserControllerTests;

[Collection(CollectionFixturesNames.DefaultIntegrationTestsCollection)]
public class SignInTests : DefaultIntegrationTests<AuthDbContext>
{
    private const string _url = "/api/users";
    
    public SignInTests(DefaultTestingWebApplicationFactory<AuthDbContext> factory) : base(factory) { }

    [Fact]
    public async Task Should_Persist_Valid_New_User()
    {
        var body = new SignInRequest
        {
            Email = "test@example.com",
            Name = "Joshua",
            Password = "ValidPassword123!"
        };
        var response = await _client.PostAsJsonAsync(_url, body);
        
        Assert.Equal(System.Net.HttpStatusCode.Created, response.StatusCode);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
        var user = await db.Users.SingleAsync(u => u.Email.Value.Equals(body.Email));
        Assert.NotNull(user);
        Assert.NotEqual(user.PasswordHash, body.Password);

        var rowVersion = db.Entry(user).Property("RowVersion").CurrentValue;
        Assert.NotEqual(Guid.Empty, rowVersion);
    }

    [Fact]
    public async Task Should_Respond_With_BadRequest_When_Email_Format_Is_Invalid()
    {
        var requestBody = new SignInRequest
        {
            Email = "invalid-email",
            Name = "Joshua",
            Password = "ValidPassword123!"
        };
        var response = await _client.PostAsJsonAsync(_url, requestBody);

        List<DetailedApiError> expectedErrors = [
            new(
                generalError: EmailResponseErrors.Format,
                field: nameof(requestBody.Email).FromPascalCaseToCamelCase(),
                location: ErrorLocation.Body,
                rejectedValue: requestBody.Email)
        ];

        await BadRequestAssertions.AssertResponse(response, _url, expectedErrors);
    }

    [Fact]
    public async Task Should_Respond_With_BadRequest_When_Email_Already_Registered()
    {
        var email = "joshua@example.com";
        var requestBody = new SignInRequest
        {
            Email = email,
            Name = "Joshua",
            Password = "ValidPassword123!"
        };
        var response = await _client.PostAsJsonAsync(_url, requestBody);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        requestBody = new SignInRequest
        {
            Email = email,
            Name = "Joshua II",
            Password = "AnotherValidPassword123!"
        };
        response = await _client.PostAsJsonAsync(_url, requestBody);

        List<DetailedApiError> expectedErrors = [
            new(
                generalError: EmailResponseErrors.AlreadyRegistered,
                field: nameof(requestBody.Email).FromPascalCaseToCamelCase(),
                location: ErrorLocation.Body,
                rejectedValue: requestBody.Email)
        ];

        await BadRequestAssertions.AssertResponse(response, _url, expectedErrors);
    }

    [Fact]
    public async Task Should_Respond_With_BadRequest_When_Name_Is_Empty()
    {
        var requestBody = new SignInRequest
        {
            Email = "test@example.com",
            Name = "",
            Password = "ValidPassword123!"
        };
        var response = await _client.PostAsJsonAsync(_url, requestBody);

        List<DetailedApiError> expectedErrors = [
            new(
                generalError: TextResponseErrors.Empty("Full name"),
                field: nameof(requestBody.Name).FromPascalCaseToCamelCase(),
                location: ErrorLocation.Body,
                rejectedValue: requestBody.Name)
        ];

        await BadRequestAssertions.AssertResponse(response, _url, expectedErrors);
    }

    [Fact]
    public async Task Should_Respond_With_BadRequest_When_Name_Is_Too_Long()
    {
        var requestBody = new SignInRequest
        {
            Email = "test@example.com",
            Name = new string('A', 101),
            Password = "ValidPassword123!"
        };
        var response = await _client.PostAsJsonAsync(_url, requestBody);

        List<DetailedApiError> expectedErrors = [
            new(
                generalError: TextResponseErrors.TooLong(100, "Full name"),
                field: nameof(requestBody.Name).FromPascalCaseToCamelCase(),
                location: ErrorLocation.Body,
                rejectedValue: requestBody.Name)
        ];

        await BadRequestAssertions.AssertResponse(response, _url, expectedErrors);
    }

    [Fact]
    public async Task Should_Respond_With_BadRequest_When_Name_Has_Only_Special_Characters()
    {
        var requestBody = new SignInRequest
        {
            Email = "test@example.com",
            Name = "!@#$%^&*()-+",
            Password = "ValidPassword123!"
        };
        var response = await _client.PostAsJsonAsync(_url, requestBody);

        List<DetailedApiError> expectedErrors = [
            new(
                generalError: TextResponseErrors.OnlySpecialCharacters("Full name"),
                field: nameof(requestBody.Name).FromPascalCaseToCamelCase(),
                location: ErrorLocation.Body,
                rejectedValue: requestBody.Name)
        ];

        await BadRequestAssertions.AssertResponse(response, _url, expectedErrors);
    }

    [Fact]
    public async Task Should_Respond_With_BadRequest_When_Password_Too_Short()
    {
        var requestBody = new SignInRequest
        {
            Email = "test@example.com",
            Name = "Joshua",
            Password = "aA1!"
        };
        var response = await _client.PostAsJsonAsync(_url, requestBody);

        List<DetailedApiError> expectedErrors = [
            new(
                TextResponseErrors.TooSmall(6, "Password"),
                field: nameof(requestBody.Password).FromPascalCaseToCamelCase(),
                location: ErrorLocation.Body,
                rejectedValue: null)
        ];

        await BadRequestAssertions.AssertResponse(response, _url, expectedErrors);
    }

    [Fact]
    public async Task Should_Respond_With_BadRequest_When_Password_Too_Long()
    {
        var requestBody = new SignInRequest
        {
            Email = "test@example.com",
            Name = "Joshua",
            Password = $"{new string('A', 101)}!a1"
        };
        var response = await _client.PostAsJsonAsync(_url, requestBody);

        List<DetailedApiError> expectedErrors = [
            new(
                generalError: TextResponseErrors.TooLong(100, "Password"),
                field: nameof(requestBody.Password).FromPascalCaseToCamelCase(),
                location: ErrorLocation.Body,
                rejectedValue: null)
        ];

        await BadRequestAssertions.AssertResponse(response, _url, expectedErrors);
    }

    [Fact]
    public async Task Should_Respond_With_BadRequest_When_Password_Contains_No_Special_Characters()
    {
        var requestBody = new SignInRequest
        {
            Email = "test@example.com",
            Name = "Joshua",
            Password = "Password123"
        };
        var response = await _client.PostAsJsonAsync(_url, requestBody);

        List<DetailedApiError> expectedErrors = [
            new(
                generalError: TextResponseErrors.AtLeastOneSpecialCharacter("Password"),
                field: nameof(requestBody.Password).FromPascalCaseToCamelCase(),
                location: ErrorLocation.Body,
                rejectedValue: null)
        ];

        await BadRequestAssertions.AssertResponse(response, _url, expectedErrors);
    }

    [Fact]
    public async Task Should_Respond_With_BadRequest_When_Password_Contains_Only_Upper_Case_Letters()
    {
        var requestBody = new SignInRequest
        {
            Email = "test@example.com",
            Name = "Joshua",
            Password = "PASSWORD123!"
        };
        var response = await _client.PostAsJsonAsync(_url, requestBody);

        List<DetailedApiError> expectedErrors = [
            new(
                generalError: TextResponseErrors.AtLeastOneUpperAndLowerCase("Password"),
                field: nameof(requestBody.Password).FromPascalCaseToCamelCase(),
                location: ErrorLocation.Body,
                rejectedValue: null)
        ];

        await BadRequestAssertions.AssertResponse(response, _url, expectedErrors);
    }

    [Fact]
    public async Task Should_Respond_With_BadRequest_When_Password_Contains_Only_Lower_Case_Letters()
    {
        var requestBody = new SignInRequest
        {
            Email = "test@example.com",
            Name = "Joshua",
            Password = "password123!"
        };
        var response = await _client.PostAsJsonAsync(_url, requestBody);

        List<DetailedApiError> expectedErrors = [
            new(
                generalError: TextResponseErrors.AtLeastOneUpperAndLowerCase("Password"),
                field: nameof(requestBody.Password).FromPascalCaseToCamelCase(),
                location: ErrorLocation.Body,
                rejectedValue: null)
        ];

        await BadRequestAssertions.AssertResponse(response, _url, expectedErrors);
    }

    [Fact]
    public async Task Should_Respond_With_400_And_Null_Value_Error_For_Each_Field_When_There_Is_No_Request_Body()
    {
        var response = await _client.PostAsync(_url, null);

        List<DetailedApiError> expectedErrors = [
            new(
                generalError: NullResponseErrors.NullValue,
                field: nameof(SignInRequest.Email).FromPascalCaseToCamelCase(),
                location: ErrorLocation.Body,
                rejectedValue: null),
            new(
                generalError: NullResponseErrors.NullValue,
                field: nameof(SignInRequest.Password).FromPascalCaseToCamelCase(),
                location: ErrorLocation.Body,
                rejectedValue: null),
            new(
                generalError: NullResponseErrors.NullValue,
                field: nameof(SignInRequest.Name).FromPascalCaseToCamelCase(),
                location: ErrorLocation.Body,
                rejectedValue: null)
        ];

        await BadRequestAssertions.AssertResponse(response, _url, expectedErrors);
    }

    [Fact]
    public async Task Should_Respond_With_BadRequest_When_Password_Contains_No_Numbers()
    {
        var requestBody = new SignInRequest
        {
            Email = "test@example.com",
            Name = "Joshua",
            Password = "Password!"
        };
        var response = await _client.PostAsJsonAsync(_url, requestBody);

        List<DetailedApiError> expectedErrors = [
            new(
                generalError: TextResponseErrors.AtLeastOneNumber("Password"),
                field: nameof(requestBody.Password).FromPascalCaseToCamelCase(),
                location: ErrorLocation.Body,
                rejectedValue: null)
        ];

        await BadRequestAssertions.AssertResponse(response, _url, expectedErrors);
    }
}