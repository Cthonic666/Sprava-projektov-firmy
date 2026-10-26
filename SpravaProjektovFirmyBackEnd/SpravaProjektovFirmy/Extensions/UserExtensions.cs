using System.Security.Claims;

namespace SpravaProjektovFirmy.Extensions;

public static class UserExtensions
{
    public static int UserId(this ClaimsPrincipal user) =>
        int.Parse(user.FindFirstValue(ClaimTypes.NameIdentifier)!);
}