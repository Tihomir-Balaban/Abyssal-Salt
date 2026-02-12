using System.Security.Claims;

namespace AbySalto.Mid.Statics;

internal static class UserContext
{
    internal static Guid GetUserId(ClaimsPrincipal user)
    {
        return GetUserIdOrNull(user)
               ?? throw new UnauthorizedAccessException("Missing or invalid user id claim (NameIdentifier).");
    }

    internal static Guid? GetUserIdOrNull(ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var id) ? id : (Guid?)null;
    }
}