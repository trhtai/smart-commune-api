using ErrorOr;

using MediatR;

using Microsoft.EntityFrameworkCore;

using SmartCommune.Application.Common.Interfaces.Persistence;
using SmartCommune.Application.Common.Interfaces.Services;
using SmartCommune.Domain.Common.Errors;
using SmartCommune.Domain.MenuItemAggregate;
using SmartCommune.Domain.MenuItemAggregate.ValueObjects;
using SmartCommune.Domain.PermissionAggregate.ValueObjects;

namespace SmartCommune.Application.Services.Manage.MenuItems.Commands.CreateMenuItem;

public class CreateMenuItemCommandHandler(
    IApplicationDbContext dbContext,
    ICacheService cacheService)
    : IRequestHandler<CreateMenuItemCommand, ErrorOr<Guid>>
{
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<ErrorOr<Guid>> Handle(CreateMenuItemCommand request, CancellationToken cancellationToken)
    {
        // 1. Validate ParentId (Nếu có truyền lên thì phải tồn tại).
        MenuItemId? parentId = null;
        if (request.ParentId.HasValue)
        {
            parentId = MenuItemId.Create(request.ParentId.Value);
            bool parentExists = await _dbContext.MenuItems
                .AnyAsync(m => m.Id == parentId, cancellationToken);

            if (!parentExists)
            {
                return Errors.Menu.ParentNotFound;
            }
        }

        // 2. Tạo ValueObject Config.
        var config = MenuItemConfig.Create(
            request.Type,
            request.Path,
            request.Icon,
            request.ActiveIcon,
            request.CheckRoutes,
            request.RelatedPaths);

        // 3. Tạo Entity MenuItem.
        var menuItem = MenuItem.Create(
            request.Label,
            request.SortOrder,
            config,
            parentId);

        // 4. Gán Permissions.
        if (request.PermissionIds.Count > 0)
        {
            var permissionIds = request.PermissionIds
                .Select(PermissionId.Create)
                .ToList();

            menuItem.UpdatePermissions(permissionIds);
        }

        // 5. Lưu vào DB.
        await _dbContext.MenuItems.AddAsync(menuItem, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // 6. Xóa Cache Menu (Bắt buộc)
        // Vì menu thay đổi cấu trúc, cần xóa cache để User tải lại menu mới.
        await _cacheService.RemoveByPrefixAsync("app:menu:", cancellationToken);

        return menuItem.Id.Value;
    }
}