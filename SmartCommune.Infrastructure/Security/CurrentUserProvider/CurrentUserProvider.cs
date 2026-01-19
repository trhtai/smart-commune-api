using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

using Microsoft.AspNetCore.Http;

using SmartCommune.Application.Common.Constants;

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
        var roleId = GetSingleClaimValue(user, CustomClaims.RoleId);
        var fullName = GetSingleClaimValue(user, JwtRegisteredClaimNames.Name);

        return new CurrentUser(Guid.Parse(id), fullName, Guid.Parse(roleId));
    }

    private static string GetSingleClaimValue(ClaimsPrincipal principal, string claimType)
    {
        var claim = principal.Claims.SingleOrDefault(c => c.Type == claimType)
            ?? throw new InvalidOperationException($"Missing claim '{claimType}' from token.");
        return claim.Value;
    }
}