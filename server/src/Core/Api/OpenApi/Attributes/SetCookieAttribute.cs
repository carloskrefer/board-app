using Microsoft.Net.Http.Headers;

namespace Core.Api.OpenApi.Attributes;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
public class SetCookieAttribute : Attribute
{
    public int WhenStatusCode { get; }
    public string Key { get; }
    public string Value { get; }
    public string? Description { get; }
    public bool IsHttpOnly { get; }
    public bool IsSecure { get; }
    public SameSiteMode SameSiteMode { get; }
    public string? Path { get; }
    public int DaysExpiration { get; }
    public bool Required { get; }
    public bool Deprecated { get; }

    public SetCookieAttribute(
        int whenStatusCode, 
        string key,
        string value,
        string? description = null,
        bool isHttpOnly = false,
        bool isSecure = false,
        SameSiteMode sameSiteMode = SameSiteMode.Lax,
        int daysExpiration = -1,
        string? path = null,
        bool required = false,
        bool deprecated = false)
    {
        WhenStatusCode = whenStatusCode;
        Key = key;
        Value = value;
        Description = description;
        IsHttpOnly = isHttpOnly;
        IsSecure = isSecure;
        SameSiteMode = sameSiteMode;
        DaysExpiration = daysExpiration;
        Path = path;
        Required = required;
        Deprecated = deprecated;
    }

    public string GetHeader()
    {
        var cookieParts = new List<string> { $"Set-Cookie: {Key}={Value}" };

        if (IsHttpOnly)
            cookieParts.Add("HttpOnly");

        if (IsSecure)
            cookieParts.Add("Secure");

        if (SameSiteMode != SameSiteMode.Unspecified)
            cookieParts.Add($"SameSite={SameSiteMode}");

        if (DaysExpiration >= 0)
            cookieParts.Add($"Expiration=<date-after-{DaysExpiration}-days>");

        if (!string.IsNullOrEmpty(Path))
            cookieParts.Add($"Path={Path}");

        return string.Join("; ", cookieParts);
    }
}