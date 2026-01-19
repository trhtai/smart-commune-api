using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using SmartCommune.Application.Common.Constants;
using SmartCommune.Application.Common.Interfaces.Persistence;
using SmartCommune.Application.Common.Interfaces.Services;
using SmartCommune.Domain.RoleAggregate.ValueObjects;

namespace SmartCommune.Infrastructure.Authorization;

public class PermissionAuthorizationHandler(
    IServiceScopeFactory serviceScopeFactory,
    ICacheService cacheService)
    : AuthorizationHandler<PermissionRequirement>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        // 1. Lấy RoleId từ User Claims.
        var roleIdClaim = context.User.FindFirst(CustomClaims.RoleId);
        if (roleIdClaim is null)
        {
            return;
        }

        string roleId = roleIdClaim.Value;
        string cacheKey = $"auth:permissions:role:{roleId}";

        // 2. Lấy Permission từ Redis
        HashSet<string>? permissions = await cacheService.GetAsync<HashSet<string>>(cacheKey);

        if (permissions is null)
        {
            // 3. Cache Miss -> Query DB.
            using var scope = serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();

            permissions = await dbContext.RolePermissions
                .Where(rp => rp.RoleId == RoleId.Create(Guid.Parse(roleId)))
                .Select(rp => rp.Permission.Code)
                .ToHashSetAsync(); // Dùng HashSet tra cứu cho nhanh O(1).

            // 4. Lưu Cache (TTL dài, ví dụ 1 tuần vì Role ít đổi).
            await cacheService.SetAsync(cacheKey, permissions, TimeSpan.FromDays(7));
        }

        // 5. Check quyền.
        if (permissions.Contains(requirement.PermissionName))
        {
            context.Succeed(requirement);
        }
    }
}