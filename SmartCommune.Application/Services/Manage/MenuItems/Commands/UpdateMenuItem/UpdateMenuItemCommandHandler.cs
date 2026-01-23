using ErrorOr;

using MediatR;

using Microsoft.EntityFrameworkCore;

using SmartCommune.Application.Common.Interfaces.Persistence;
using SmartCommune.Application.Common.Interfaces.Services;
using SmartCommune.Domain.Common.Errors;
using SmartCommune.Domain.MenuItemAggregate.ValueObjects;
using SmartCommune.Domain.PermissionAggregate.ValueObjects;

namespace SmartCommune.Application.Services.Manage.MenuItems.Commands.UpdateMenuItem;

public class UpdateMenuItemCommandHandler(
    IApplicationDbContext dbContext,
    ICacheService cacheService)
    : IRequestHandler<UpdateMenuItemCommand, ErrorOr<Updated>>
{
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<ErrorOr<Updated>> Handle(UpdateMenuItemCommand request, CancellationToken cancellationToken)
    {
        var menuItemId = MenuItemId.Create(request.Id);
        var menuItem = await _dbContext.MenuItems
            .Include(m => m.Permissions)
            .FirstOrDefaultAsync(x => x.Id == menuItemId, cancellationToken);

        if (menuItem is null)
        {
            return Errors.Menu.NotFound;
        }

        // Tạo ValueObject Config mới.
        var config = MenuItemConfig.Create(
            request.Type,
            request.Path,
            request.Icon,
            request.ActiveIcon,
            request.CheckRoutes,
            request.RelatedPaths);

        // Gọi method Update trong Domain.
        menuItem.Update(
            request.Label,
            menuItem.SortOrder,
            config,
            menuItem.ParentId);

        // Update Permissions.
        if (request.PermissionIds is not null)
        {
            var permissionIds = request.PermissionIds
                .Select(PermissionId.Create)
                .ToList();

            menuItem.UpdatePermissions(permissionIds);
        }

        _dbContext.MenuItems.Update(menuItem);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Vì menu thay đổi cấu trúc, cần xóa cache để User tải lại menu mới.
        await _cacheService.RemoveByPrefixAsync("app:menu:", cancellationToken);

        return Result.Updated;
    }
}