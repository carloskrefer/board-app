using Microsoft.AspNetCore.Http;

namespace Core.Api.Authentication;

public static class CookieExtensions
{
    public static Guid GetRefreshToken(this IRequestCookieCollection cookies)
    {
        var cookie = cookies["refreshToken"];

        if (cookie is null)
            throw new InvalidOperationException("Cookie \"refreshToken\" not found.");

        return Guid.Parse(cookie);
    }
}