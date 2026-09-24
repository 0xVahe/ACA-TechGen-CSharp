using System.Security.Claims;

namespace ToDoApp.Auth;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal user)
    {
        var value = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(value) || !Guid.TryParse(value, out var id))
        {
            throw new InvalidOperationException("User identity missing or invalid.");
        }
        return id;
    }
}