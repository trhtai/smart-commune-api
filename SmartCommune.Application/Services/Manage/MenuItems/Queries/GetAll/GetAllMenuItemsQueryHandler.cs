using ErrorOr;

using MediatR;

using Microsoft.EntityFrameworkCore;

using SmartCommune.Application.Common.Interfaces.Persistence;
using SmartCommune.Application.Services.Manage.MenuItems.Common;
using SmartCommune.Domain.MenuItemAggregate;

namespace SmartCommune.Application.Services.Manage.MenuItems.Queries.GetAll;

public class GetAllMenuItemsQueryHandler(
    IApplicationDbContext dbContext)
    : IRequestHandler<GetAllMenuItemsQuery, ErrorOr<List<MenuItemManageResult>>>
{
    public async Task<ErrorOr<List<MenuItemManageResult>>> Handle(GetAllMenuItemsQuery request, CancellationToken cancellationToken)
    {
        // 1. Load Full Data (Kèm Permissions).
        var allItems = await dbContext.MenuItems
            .AsNoTracking()
            .Include(m => m.Permissions)
            .OrderBy(m => m.SortOrder)
            .ToListAsync(cancellationToken);

        // 2. Build Tree.
        var tree = BuildAdminTree(allItems);

        return tree;
    }

    private List<MenuItemManageResult> BuildAdminTree(List<MenuItem> flatItems)
    {
        var lookup = flatItems.ToDictionary(x => x.Id, x => new MenuItemManageResult(
            x.Id.Value,
            x.Label,
            x.Config.Icon,
            x.Config.ActiveIcon,
            x.Config.Path,
            x.Config.Type,
            x.SortOrder,
            x.ParentId?.Value,
            x.Config.CheckRoutes,
            x.Config.RelatedPaths,
            [.. x.Permissions.Select(p => p.PermissionId.Value)],
            []));

        var rootNodes = new List<MenuItemManageResult>();

        foreach (var item in flatItems)
        {
            if (!lookup.TryGetValue(item.Id, out var dto))
            {
                continue;
            }

            if (item.ParentId is null)
            {
                rootNodes.Add(dto);
            }
            else
            {
                if (lookup.TryGetValue(item.ParentId, out var parentDto))
                {
                    parentDto.Children.Add(dto);
                }
            }
        }

        return [.. rootNodes.OrderBy(x => x.OrderIndex)];
    }
}