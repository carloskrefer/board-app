using System.Net.Http.Json;
using Core.Api.Errors;

namespace TestsCommon.Assertions;

public static class BadRequestAssertions
{
    /// <summary>
    /// Asserts a "general response", meaning it has an empty errors array (i.e. the specific application error is only
    /// described by a specific title and detail property values).
    /// </summary>
    public static async Task AssertGeneralResponse(
        HttpResponseMessage response,
        string expectedInstance,
        GeneralResponseError expectedError)
    {
        var body = await AssertResponseExceptErrorsArrayAndReturnBody(
            response, 
            expectedInstance, 
            expectedError.Code, 
            expectedError.Message);
        
        Assert.Empty(body.Errors);
    }

    /// <summary>
    /// Asserts a "detailed response", meaning the errors array contains one or more detailed errors, while the
    /// title and detail properties are set to the default values.
    /// </summary>
    public static async Task AssertDetailedResponse(
        HttpResponseMessage response,
        string expectedInstance,
        IEnumerable<DetailedResponseError> expectedErrors)
    {
        var body = await AssertResponseExceptErrorsArrayAndReturnBody(response, expectedInstance);
        AssertErrorsArray(expectedErrors, body.Errors);
    }

    public static void AssertErrorsArray(
        IEnumerable<DetailedResponseError> expectedErrors,
        IEnumerable<DetailedResponseError> actualErrors)
    {
        Assert.True(expectedErrors.Order().SequenceEqual(actualErrors.Order()));
    }

    public static async Task<ApiProblemDetails> AssertResponseExceptErrorsArrayAndReturnBody(
        HttpResponseMessage response,
        string expectedInstance,
        string expectedTitle = "One or more validation errors occurred.",
        string expectedDetail = "See the errors property for details.")
    {
        var statusCode = response.StatusCode;
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, statusCode);

        var contentType = response.Content.Headers.GetValues("Content-Type").FirstOrDefault();
        Assert.Contains("application/problem+json", contentType);

        var body = await response.Content.ReadFromJsonAsync<ApiProblemDetails>();
        Assert.NotNull(body);
        Assert.Equal(expectedTitle, body.Title);
        Assert.Equal(expectedDetail, body.Detail);
        Assert.Equal((int) statusCode, body.Status);
        Assert.Equal(expectedInstance, body.Instance);

        var traceId = response.Headers.GetValues("X-Trace-Id").FirstOrDefault();
        Assert.Equal(traceId, body.TraceId);
        
        return body;
    }
}