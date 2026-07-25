using System.Net.Http.Json;
using Auth.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestsCommon.Persistance.Factories;
using TestsCommon.Persistance.Base;
using Auth.Api.Tests.CollectionFixtures;
using Core.Api.Errors;
using Core.String.Extensions;
using TestsCommon.Assertions;
using TestsCommon.Tests;
using Core.Api.DTOs.Auth.User;

namespace Auth.Api.Tests.IntegrationTests.UserControllerTests;

[Collection(CollectionFixturesNames.DefaultIntegrationTestsCollection)]
public class SignInTests : DefaultIntegrationTests<AuthDbContext>
{
    private const string _url = "/api/users";
    
    public SignInTests(DefaultTestingWebApplicationFactory<AuthDbContext> factory) : base(factory) { }

    [Fact]
    public async Task Should_Persist_Valid_New_User()
    {
        var body = new SignInRequest("test@example.com", "Joshua", "ValidPassword123!");
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
        var requestBody = new SignInRequest("invalid-email", "Joshua", "ValidPassword123!");
        var response = await _client.PostAsJsonAsync(_url, requestBody);

        List<DetailedResponseError> expectedErrors = [
            new(
                generalError: EmailResponseErrors.Format,
                field: nameof(requestBody.Email).FromPascalCaseToCamelCase(),
                location: ErrorLocationEnum.Body,
                rejectedValue: requestBody.Email)
        ];

        await BadRequestAssertions.AssertDetailedResponse(response, _url, expectedErrors);
    }

    [Fact]
    public async Task Should_Respond_With_BadRequest_When_Name_Is_Empty()
    {
        var requestBody = new SignInRequest("test@example.com", "", "ValidPassword123!");
        var response = await _client.PostAsJsonAsync(_url, requestBody);

        List<DetailedResponseError> expectedErrors = [
            new(
                generalError: TextResponseErrors.Empty,
                field: nameof(requestBody.Name).FromPascalCaseToCamelCase(),
                location: ErrorLocationEnum.Body,
                rejectedValue: requestBody.Name)
        ];

        await BadRequestAssertions.AssertDetailedResponse(response, _url, expectedErrors);
    }

    [Fact]
    public async Task Should_Respond_With_BadRequest_When_Name_Is_Too_Long()
    {
        var requestBody = new SignInRequest("test@example.com", new string('A', 101), "ValidPassword123!");
        var response = await _client.PostAsJsonAsync(_url, requestBody);

        List<DetailedResponseError> expectedErrors = [
            new(
                generalError: TextResponseErrors.TooLong(100),
                field: nameof(requestBody.Name).FromPascalCaseToCamelCase(),
                location: ErrorLocationEnum.Body,
                rejectedValue: requestBody.Name)
        ];

        await BadRequestAssertions.AssertDetailedResponse(response, _url, expectedErrors);
    }

    [Fact]
    public async Task Should_Respond_With_BadRequest_When_Name_Has_Only_Special_Characters()
    {
        var requestBody = new SignInRequest("test@example.com", "!@#$%^&*()-+", "ValidPassword123!");
        var response = await _client.PostAsJsonAsync(_url, requestBody);

        List<DetailedResponseError> expectedErrors = [
            new(
                generalError: TextResponseErrors.OnlySpecialCharacters,
                field: nameof(requestBody.Name).FromPascalCaseToCamelCase(),
                location: ErrorLocationEnum.Body,
                rejectedValue: requestBody.Name)
        ];

        await BadRequestAssertions.AssertDetailedResponse(response, _url, expectedErrors);
    }

    [Fact]
    public async Task Should_Respond_With_BadRequest_When_Password_Too_Short()
    {
        var requestBody = new SignInRequest("test@example.com", "Joshua", "aA1!");
        var response = await _client.PostAsJsonAsync(_url, requestBody);

        List<DetailedResponseError> expectedErrors = [
            new(
                code: $"{nameof(SignInRequest.Password).ToUpper()}.LENGTH",
                message: "Password must be between 6 and 100 characters.",
                field: nameof(requestBody.Password).FromPascalCaseToCamelCase(),
                location: ErrorLocationEnum.Body,
                rejectedValue: requestBody.Password)
        ];

        await BadRequestAssertions.AssertDetailedResponse(response, _url, expectedErrors);
    }

    [Fact]
    public async Task Should_Respond_With_BadRequest_When_Password_Too_Long()
    {
        var requestBody = new SignInRequest("test@example.com", "Joshua", $"{new string('A', 101)}!a1");
        var response = await _client.PostAsJsonAsync(_url, requestBody);

        List<DetailedResponseError> expectedErrors = [
            new(
                code: $"{nameof(SignInRequest.Password).ToUpper()}.LENGTH",
                message: "Password must be between 6 and 100 characters.",
                field: nameof(requestBody.Password).FromPascalCaseToCamelCase(),
                location: ErrorLocationEnum.Body,
                rejectedValue: requestBody.Password)
        ];

        await BadRequestAssertions.AssertDetailedResponse(response, _url, expectedErrors);
    }

    [Fact]
    public async Task Should_Respond_With_BadRequest_When_Password_Contains_No_Special_Characters()
    {
        var requestBody = new SignInRequest("test@example.com", "Joshua", "Password123");
        var response = await _client.PostAsJsonAsync(_url, requestBody);

        List<DetailedResponseError> expectedErrors = [
            new(
                code: $"{nameof(SignInRequest.Password).ToUpper()}.SPECIAL_CHARACTERS",
                message: "Password must contain at least one special character. Examples: !?@#$%^&*()-+.",
                field: nameof(requestBody.Password).FromPascalCaseToCamelCase(),
                location: ErrorLocationEnum.Body,
                rejectedValue: requestBody.Password)
        ];

        await BadRequestAssertions.AssertDetailedResponse(response, _url, expectedErrors);
    }

    [Fact]
    public async Task Should_Respond_With_BadRequest_When_Password_Contains_Only_Upper_Case_Letters()
    {
        var requestBody = new SignInRequest("test@example.com", "Joshua", "PASSWORD123!");
        var response = await _client.PostAsJsonAsync(_url, requestBody);

        List<DetailedResponseError> expectedErrors = [
            new(
                code: $"{nameof(SignInRequest.Password).ToUpper()}.UPPER_AND_LOWER_CASE",
                message: "Password must contain at least one uppercase and one lowercase letter.",
                field: nameof(requestBody.Password).FromPascalCaseToCamelCase(),
                location: ErrorLocationEnum.Body,
                rejectedValue: requestBody.Password)
        ];

        await BadRequestAssertions.AssertDetailedResponse(response, _url, expectedErrors);
    }

    [Fact]
    public async Task Should_Respond_With_BadRequest_When_Password_Contains_Only_Lower_Case_Letters()
    {
        var requestBody = new SignInRequest("test@example.com", "Joshua", "password123!");
        var response = await _client.PostAsJsonAsync(_url, requestBody);

        List<DetailedResponseError> expectedErrors = [
            new(
                code: $"{nameof(SignInRequest.Password).ToUpper()}.UPPER_AND_LOWER_CASE",
                message: "Password must contain at least one uppercase and one lowercase letter.",
                field: nameof(requestBody.Password).FromPascalCaseToCamelCase(),
                location: ErrorLocationEnum.Body,
                rejectedValue: requestBody.Password)
        ];

        await BadRequestAssertions.AssertDetailedResponse(response, _url, expectedErrors);
    }

    [Fact]
    public async Task Should_Respond_With_BadRequest_When_Password_Contains_No_Numbers()
    {
        var requestBody = new SignInRequest("test@example.com", "Joshua", "Password!");
        var response = await _client.PostAsJsonAsync(_url, requestBody);

        List<DetailedResponseError> expectedErrors = [
            new(
                code: $"{nameof(SignInRequest.Password).ToUpper()}.NUMBER", 
                message: "Password must contain at least one number.",
                field: nameof(requestBody.Password).FromPascalCaseToCamelCase(),
                location: ErrorLocationEnum.Body,
                rejectedValue: requestBody.Password)
        ];

        await BadRequestAssertions.AssertDetailedResponse(response, _url, expectedErrors);
    }

    [Fact]
    public async Task Should_Respond_With_406_When_Accept_Header_Is_Not_Application_Json() =>
        await JsonTests.ResponseShouldBe406WhenAcceptHeaderIsNotApplicationJson(
            _url, 
            _client,
            HttpMethod.Post,
            new SignInRequest("test@example.com", "Joshua", "ValidPassword123!"));
}