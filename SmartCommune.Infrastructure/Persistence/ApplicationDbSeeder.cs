using Microsoft.EntityFrameworkCore;

using SmartCommune.Application.Common.Constants;
using SmartCommune.Application.Common.Interfaces.Persistence;
using SmartCommune.Application.Common.Interfaces.Services;
using SmartCommune.Domain.PermissionAggregate;
using SmartCommune.Domain.RoleAggregate;
using SmartCommune.Domain.UserAggregate;

namespace SmartCommune.Infrastructure.Persistence;

public class ApplicationDbSeeder(
    IApplicationDbContext dbContext,
    IDateTimeProvider dateTimeProvider)
{
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly IDateTimeProvider _dateTimeProvider = dateTimeProvider;

    public async Task SeedAsync(CancellationToken cancellationToken)
    {
        await SeedRolesAsync(cancellationToken);
        await SeedPermissionsAsync(cancellationToken);
        await SeedAdminUserAsync(cancellationToken);
        await SeedAdminRoleWithDefaultPermissionsAsync(cancellationToken);
    }

    private async Task SeedPermissionsAsync(CancellationToken cancellationToken)
    {
        var predefinedPermissions = new List<(string Code, string Name)>
        {
            (PermissionCodes.Permission.ViewAll, "Xem tất cả quyền"),
        };

        var permissions = new List<Permission>();

        foreach (var (code, name) in predefinedPermissions)
        {
            var exists = await _dbContext.Permissions.AnyAsync(p => p.Code == code, cancellationToken);
            if (!exists)
            {
                var permission = Permission.Create(code, name);
                permissions.Add(permission);
            }
        }

        if (permissions.Count > 0)
        {
            await _dbContext.Permissions.AddRangeAsync(permissions, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    // Gán toàn bộ quyền hiện có cho role Admin.
    private async Task SeedAdminRoleWithDefaultPermissionsAsync(CancellationToken cancellationToken)
    {
        // Lấy role Admin cùng với danh sách quyền hiện có.
        var adminRole = await _dbContext.Roles
            .Include(r => r.Permissions)
            .FirstOrDefaultAsync(r => r.Code == RoleCodes.Admin, cancellationToken);

        if (adminRole is not null)
        {
            // Lấy tất cả quyền đang có trong hệ thống.
            var allPermissions = await _dbContext.Permissions
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            // Lấy tất cả Permission Ids đã có trong Admin và lưu vào HashSet.
            var existingPermissionIds = adminRole.Permissions
                .Select(rp => rp.PermissionId)
                .ToHashSet(); // Sử dụng HashSet để lặp nhanh hơn.

            // Lọc ra các quyền còn thiếu của Admin.
            var missingPermissions = allPermissions
                .Where(p => !existingPermissionIds.Contains(p.Id))
                .ToList();

            // Thêm các quyền còn thiếu vào role Admin.
            if (missingPermissions.Count > 0)
            {
                foreach (var permission in missingPermissions)
                {
                    adminRole.GrantPermission(permission.Id);
                }

                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }

    private async Task SeedRolesAsync(CancellationToken cancellationToken)
    {
        var predefinedRoles = new List<(string Code, string Name)>
        {
            (RoleCodes.Admin, "Quản trị viên"),
        };

        var roles = new List<Role>();

        foreach (var (code, name) in predefinedRoles)
        {
            var exists = await _dbContext.Roles.AnyAsync(r => r.Code == code, cancellationToken);
            if (!exists)
            {
                var role = Role.Create(code, name);
                roles.Add(role);
            }
        }

        if (roles.Count > 0)
        {
            await _dbContext.Roles.AddRangeAsync(roles, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task SeedAdminUserAsync(CancellationToken cancellationToken)
    {
        var adminRole = await _dbContext.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => r.Code == RoleCodes.Admin, cancellationToken);

        if (adminRole is not null)
        {
            bool adminExists = await _dbContext.Users.AnyAsync(u => u.RoleId == adminRole.Id, cancellationToken);
            if (!adminExists)
            {
                var adminUser = ApplicationUser.Create(
                    userName: "admin",
                    rawPassword: "cadico@8386",
                    fullName: "Quản trị viên",
                    createdAt: _dateTimeProvider.VietNamNow,
                    roleId: adminRole.Id);

                await _dbContext.Users.AddAsync(adminUser, cancellationToken);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }
}