using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace TestsCommon.Tests;

public static class JsonTests
{
    public static async Task ResponseShouldBe406WhenAcceptHeaderIsNotApplicationJson<TBody>(
        string url, 
        HttpClient client,
        HttpMethod httpMethod,
        TBody? body = null) 
        where TBody : class
    {
        var request = new HttpRequestMessage(httpMethod, url);

        if (body is not null)
            request.Content = JsonContent.Create(body);

        request.Headers.Accept.Clear();
        request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
        var response = await client.SendAsync(request);

        Assert.Equal(System.Net.HttpStatusCode.NotAcceptable, response.StatusCode);
    }
}