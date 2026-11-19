using System.Net.Http.Json;
using Contracts.Errors;
using Core.Api;

namespace TestsCommon.Assertions;

public static class BadRequestAssertions
{
    public static async Task<ApiProblemDetails> AssertResponse(
        HttpResponseMessage response,
        string expectedInstance,
        IEnumerable<DetailedApiError> expectedErrors)
    {
        var statusCode = AssertStatus(response);
        var traceId = AssertHeaders(response);
        return await AssertBodyAsync(response, expectedInstance, expectedErrors, statusCode, traceId);
    }

    private static int AssertStatus(HttpResponseMessage response)
    {
        int status = (int) response.StatusCode;
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);
        return status;
    }

    private static string AssertHeaders(HttpResponseMessage response)
    {
        AssertContentTypeHeader(response);
        return AssertTraceIdHeader(response);
    }
        

    private static void AssertContentTypeHeader(HttpResponseMessage response)
    {
        var contentType = response.Content.Headers.GetValues("Content-Type").FirstOrDefault();
        Assert.Contains("application/problem+json", contentType);   
    }

    private static async Task<ApiProblemDetails> AssertBodyAsync(
        HttpResponseMessage response, 
        string expectedInstance,
        IEnumerable<DetailedApiError> expectedErrors,
        int expectedStatusCode,
        string expectedTraceId)
    {
        var body = await response.Content.ReadFromJsonAsync<ApiProblemDetails>();

        Assert.NotNull(body);
        Assert.Equal(BadRequestConstants.Title, body.Title);
        Assert.Equal(BadRequestConstants.DetailForErrorsExtension, body.Detail);
        Assert.Equal(expectedStatusCode, body.Status);
        Assert.Equal(expectedInstance, body.Instance);
        AssertErrorsExtension(expectedErrors, body.Errors);
        Assert.Equal(expectedTraceId, body.TraceId);

        return body;
    }

    private static string AssertTraceIdHeader(HttpResponseMessage response)
    {
        var traceId = response.Headers.GetValues("X-Trace-Id").FirstOrDefault();

        Assert.NotNull(traceId);
        Assert.NotEmpty(traceId);

        return traceId;
    }

    public static void AssertErrorsExtension(
        IEnumerable<DetailedApiError> expectedErrors,
        IEnumerable<DetailedApiError> actualErrors)
    {
        var expectedOrdered = Order(expectedErrors);
        var actualOrdered = Order(actualErrors);
        Assert.True(expectedOrdered.SequenceEqual(actualOrdered));
    }

    private static IEnumerable<DetailedApiError> Order(IEnumerable<DetailedApiError> errors)
    {
        return errors.OrderBy(e => e.Field).ThenBy(e => e.Code).ThenBy(e => e.RejectedValue);
    }
}