using Core.Api.Authentication;
using Microsoft.AspNetCore.Http;

namespace Auth.Api.Utils;

public static class RefreshTokenUtils
{
    public static void AddRefreshTokenCookie(this IResponseCookies cookies, Guid sessionId, string path)
    {
        cookies.Append(
            key: RefreshTokenConstants.CookieKey, 
            value: sessionId.ToString(), 
            options: CreateRefreshTokenCookieOptions(path));
    }

    private static CookieOptions CreateRefreshTokenCookieOptions(string path) => 
        new()
        {
            HttpOnly = true,
            // Secure = true, // TODO: I think this should be enabled in production, test latter whenever HTTPs is working
            SameSite = SameSiteMode.Lax,
            Path = path,
            Expires = DateTime.UtcNow.AddDays(7)
        };

    public static void DeleteRefreshTokenCookie(this IResponseCookies cookies, string path)
    {
        cookies.Delete(
            key: RefreshTokenConstants.CookieKey,
            options: CreateRefreshTokenCookieOptions(path));
    }
}