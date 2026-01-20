using Microsoft.EntityFrameworkCore;

using SmartCommune.Application.Common.Interfaces.Persistence;
using SmartCommune.Application.Common.Interfaces.Services;
using SmartCommune.Domain.RoleAggregate.ValueObjects;

namespace SmartCommune.Application.Services.Identity.Permissions;

public class PermissionService(
    IApplicationDbContext dbContext,
    ICacheService cacheService)
    : IPermissionService
{
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<HashSet<string>> GetPermissionsAsync(RoleId roleId, CancellationToken cancellationToken = default)
    {
        string cacheKey = $"auth:permissions:role:{roleId.Value}";

        // Try to get permissions from cache.
        var cachedPermissions = await _cacheService.GetAsync<HashSet<string>>(cacheKey, cancellationToken);
        if (cachedPermissions is not null)
        {
            return cachedPermissions;
        }

        // Cache miss -> Fetch permissions from the database.
        var permissions = await _dbContext.RolePermissions
            .AsNoTracking()
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.Permission.Code)
            .ToHashSetAsync(cancellationToken);

        await _cacheService.SetAsync(cacheKey, permissions, TimeSpan.FromDays(7), cancellationToken);

        return permissions;
    }

    public async Task ClearCacheAsync(RoleId roleId, CancellationToken cancellationToken = default)
    {
        string cacheKey = $"auth:permissions:role:{roleId.Value}";
        await _cacheService.RemoveAsync(cacheKey, cancellationToken);
    }
}