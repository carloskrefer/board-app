using System.Net.Http.Headers;
using TestsCommon.Helpers;

namespace TestsCommon.Extensions;

public static class CookiesExtensions
{
    /// <summary>
    /// Gets the values of the "Set-Cookie" headers that start with the specified cookie name.
    /// </summary>
    /// <remarks>
    /// <example>
    /// If the following headers exist:
    /// <code>
    /// Header 1: "Set-Cookie: refreshToken=abc1; Path=/; HttpOnly;"
    /// Header 2: "Set-Cookie: refreshToken=abc2; Path=/; HttpOnly;"
    /// </code>
    /// The method will return a list containing:
    /// <code>
    /// "refreshToken=abc1; Path=/; HttpOnly;"
    /// "refreshToken=abc2; Path=/; HttpOnly;"
    /// </code>
    /// </example>
    /// </remarks>
    public static List<string> GetManySetCookieHeaderValueByName(this HttpResponseHeaders headers, string cookieName) =>
        headers
            .GetValues("Set-Cookie")
            .Where(headerValue => headerValue.StartsWith($"{cookieName}="))
            .ToList();   

    public static string SingleCookieByName(
        this HttpResponseHeaders headers, 
        string cookieName, 
        out string cookieValue, 
        out List<string> cookieAttributes)
    {
        var cookieHeaderValue = headers.GetManySetCookieHeaderValueByName(cookieName).Single();
        cookieValue = CookieHelper.GetCookieValueBySetCookieHeaderValue(cookieHeaderValue);
        cookieAttributes = CookieHelper.GetCookieAttributesFromSetCookieHeaderValue(cookieHeaderValue);
        return cookieHeaderValue;
    }         
}