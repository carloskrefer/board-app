using Microsoft.AspNetCore.Http;

namespace Auth.Api.Utils;

public static class RefreshTokenUtils
{
    public static readonly string key = "refreshToken";

    public static void AddRefreshTokenCookie(this IResponseCookies cookies, Guid sessionId)
    {
        cookies.Append(key, sessionId.ToString(), new CookieOptions
        {
            HttpOnly = true,
            // Secure = true, // TODO: I think this should be enabled in production, test latter whenever HTTPs is working
            SameSite = SameSiteMode.Lax,
            Path = "/api/users",
            Expires = DateTime.UtcNow.AddDays(7)
        });
    }

    public static void DeleteRefreshTokenCookie(this IResponseCookies cookies)
    {
        cookies.Delete(key);
    }
}