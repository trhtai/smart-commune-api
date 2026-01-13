using System.Security.Claims;

using Microsoft.AspNetCore.Http;

namespace SmartCommune.Infrastructure.Security.CurrentUserProvider;

public class CurrentUserProvider(
    IHttpContextAccessor httpContextAccessor)
    : ICurrentUserProvider
{
    public CurrentUser GetCurrentUser()
    {
        var httpContext = httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("HTTP context is not available.");

        var user = httpContext.User;

        if (user?.Identity is not { IsAuthenticated: true })
        {
            throw new InvalidOperationException("User is not authenticated.");
        }

        var id = GetSingleClaimValue(user, ClaimTypes.NameIdentifier);

        // var role = GetSingleClaimValue(user, ClaimTypes.Role);
        // var fullName = GetSingleClaimValue(user, JwtRegisteredClaimNames.Name);
        // var permissions = GetClaimValues(user, CustomClaims.Permissions);

        return new CurrentUser(Guid.Parse(id));
    }

    private static string GetSingleClaimValue(ClaimsPrincipal principal, string claimType)
    {
        var claim = principal.Claims.SingleOrDefault(c => c.Type == claimType)
            ?? throw new InvalidOperationException($"Missing claim '{claimType}' from token.");
        return claim.Value;
    }

    private static List<string> GetClaimValues(ClaimsPrincipal principal, string claimType)
    {
        return [.. principal.Claims
            .Where(c => c.Type == claimType)
            .Select(c => c.Value)];
    }
}