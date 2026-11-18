using System.Security.Claims;

namespace Core.Api.Authentication;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetSub(this ClaimsPrincipal principal)
    {
        var sub = principal.FindFirst("sub")?.Value;
        
        if (sub is null)
            throw new InvalidOperationException("Sub not found in claims.");
        
        return Guid.Parse(sub);
    }
}