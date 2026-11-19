using System.Security.Claims;

namespace Core.Api.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetSub(this ClaimsPrincipal principal)
    {
        var sub = principal.FindFirstValue(ClaimTypes.NameIdentifier);
        
        Guid.TryParse(sub, out var result);

        return result;
    }
}