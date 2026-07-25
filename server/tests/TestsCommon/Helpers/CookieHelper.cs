namespace TestsCommon.Helpers;

public static class CookieHelper
{
    /// <summary>
    /// Gets the value of the cookie from the given "Set-Cookie" header value.
    /// </summary>
    /// <remarks>
    /// <example>
    /// If the following headers value is given:
    /// <code>
    /// "refreshToken=abc123; Path=/; HttpOnly;"
    /// </code>
    /// The method will return:
    /// <code>
    /// "abc123"
    /// </code>
    /// </example>
    /// </remarks>
    public static string GetCookieValueBySetCookieHeaderValue(string setCookieHeaderValue) =>
        setCookieHeaderValue    // "refreshToken=abc123; Path=/; HttpOnly; ..."
            .Split(';')         // "refreshToken=abc123", "Path=/", "HttpOnly", ...
            .Select(attribute => attribute.Trim())
            .First()            // "refreshToken=abc123"
            .Split('=')[1];     // "abc123"

    /// <summary>
    /// Gets the individual cookie attribute from the given "Set-Cookie" header value.
    /// </summary>
    /// <remarks>
    /// <example>
    /// If the following headers value is given:
    /// <code>
    /// "refreshToken=abc123; Path=/api/auth; HttpOnly;"
    /// </code>
    /// The method will return the following, if the requested attribute is "Path":
    /// <code>
    /// "Path=/api/auth"
    /// </code>
    /// </example>
    /// </remarks>
    public static string? GetCookieAttributeBySetCookieHeaderValue(
        string setCookieHeaderValue, 
        string attribute) =>
            setCookieHeaderValue    
                .Split(';')        
                .Select(part => part.Trim())
                .FirstOrDefault(part => part.StartsWith(attribute));

    /// <summary>
    /// Gets a collection containing all cookie attributes.
    /// </summary>
    /// <remarks>
    /// <example>
    /// If the following headers value is given:
    /// <code>
    /// "refreshToken=abc123; Path=/api/auth; HttpOnly;"
    /// </code>
    /// The method will return a collection containing the following elements:
    /// <code>
    /// "Path=/api/auth"
    /// "HttpOnly"
    /// </code>
    /// </example>
    /// </remarks>
    public static List<string> GetCookieAttributesFromSetCookieHeaderValue(string setCookieHeaderValue) =>
        setCookieHeaderValue                // "refreshToken=abc123; Path=/api/auth; HttpOnly;"
            .Split(';')                     // "refreshToken=abc123", "Path=/api/auth", "HttpOnly"
            .Skip(1)                        // "Path=/api/auth", "HttpOnly"
            .Select(part => part.Trim())
            .ToList(); 

    /// <summary>
    /// Given a collection of attributes of single cookie, returns the UTC expiration from the "Expires" 
    /// attribute of the cookie.
    /// </summary>
    public static DateTime SingleExpirationUtcFromCookieAttributes(this List<string> cookieAttributes)
    {
        var expirationString = cookieAttributes
            .Single(attr => attr.StartsWith("Expires", StringComparison.OrdinalIgnoreCase))
            .Split('=', 2)[1];

        return DateTimeOffset.Parse(expirationString).UtcDateTime;
    }  
}