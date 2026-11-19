using Microsoft.AspNetCore.Http;

namespace Core.Api.Authentication;

public static class CookieExtensions
{
    public static Guid GetRefreshToken(this IRequestCookieCollection cookies)
    {
        var cookie = cookies[RefreshTokenConstants.CookieKey];

        Guid.TryParse(cookie, out var result);

        return result;
    }
}