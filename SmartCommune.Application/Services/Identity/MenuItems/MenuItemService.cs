using Microsoft.EntityFrameworkCore;

using SmartCommune.Application.Common.Interfaces.Persistence;
using SmartCommune.Application.Common.Interfaces.Services;
using SmartCommune.Domain.MenuItemAggregate;
using SmartCommune.Domain.RoleAggregate.ValueObjects;

namespace SmartCommune.Application.Services.Identity.MenuItems;

public class MenuItemService(
    IApplicationDbContext dbContext,
    ICacheService cacheService)
    : IMenuItemService
{
    private readonly IApplicationDbContext _dbContext = dbContext;
    private readonly ICacheService _cacheService = cacheService;

    public async Task<List<MenuItemResult>> GetMenuAsync(RoleId roleId, CancellationToken cancellationToken = default)
    {
        string cacheKey = $"app:menu:role:{roleId.Value}";

        // 1. Thử lấy từ Cache Redis
        var cachedMenu = await _cacheService.GetAsync<List<MenuItemResult>>(cacheKey, cancellationToken);
        if (cachedMenu is not null)
        {
            return cachedMenu;
        }

        // 2. Nếu chưa có -> Tính toán lại
        var menuTree = await BuildMenuTreeInternal(roleId, cancellationToken);

        // 3. Lưu cache (TTL 7 ngày bằng với Permissions)
        await _cacheService.SetAsync(cacheKey, menuTree, TimeSpan.FromDays(7), cancellationToken);

        return menuTree;
    }

    public async Task ClearCacheAsync(RoleId roleId, CancellationToken cancellationToken = default)
    {
        string cacheKey = $"app:menu:role:{roleId.Value}";
        await _cacheService.RemoveAsync(cacheKey, cancellationToken);
    }

    private async Task<List<MenuItemResult>> BuildMenuTreeInternal(RoleId roleId, CancellationToken cancellationToken)
    {
        // A. Lấy danh sách PermissionId mà Role này sở hữu
        // (RolePermission -> PermissionId)
        var rolePermissionIds = await _dbContext.RolePermissions
            .AsNoTracking()
            .Where(rp => rp.RoleId == roleId)
            .Select(rp => rp.PermissionId)
            .ToListAsync(cancellationToken);

        // B. Lấy toàn bộ MenuItems từ DB (Kèm danh sách Permissions yêu cầu của Menu)
        var allMenuItems = await _dbContext.MenuItems
            .AsNoTracking()
            .Include(m => m.Permissions) // Quan trọng: Include bảng MenuItemPermission
            .OrderBy(m => m.SortOrder)
            .ToListAsync(cancellationToken);

        // C. BỘ LỌC 1: CHECK QUYỀN (FLAT FILTER)
        // Chỉ giữ lại những item mà Role THỎA MÃN điều kiện về quyền.
        // Chưa quan tâm cha con, cứ lọc phẳng trước.
        var authorizedItems = allMenuItems.Where(item =>
        {
            // Trường hợp 1: Menu item KHÔNG yêu cầu quyền gì cả (Permissions rỗng)
            // -> Mặc định là cho phép hiển thị (thường là folder cha chung chung).
            if (item.Permissions.Count == 0)
            {
                return true;
            }

            // Trường hợp 2: Menu item CÓ yêu cầu quyền
            // -> Role của user phải chứa ít nhất 1 permissionId trùng với item.Permissions
            bool hasPermission = item.Permissions.Any(p => rolePermissionIds.Contains(p.PermissionId));

            return hasPermission;
        }).ToList();

        // D. DỰNG CÂY THÔ (BUILD RAW TREE)
        // Biến danh sách phẳng authorizedItems thành cây cha - con.
        var rawTree = BuildRawTree(authorizedItems);

        // E. BỘ LỌC 2: CẮT TỈA ĐỆ QUY (RECURSIVE PRUNING)
        // Loại bỏ các Folder cha bị rỗng ruột (do con cái bị filter mất ở bước C).
        var finalTree = PruneTreeRecursive(rawTree);

        return finalTree;
    }

    /// <summary>
    /// Chuyển đổi danh sách phẳng thành cây dựa trên ParentId.
    /// </summary>
    private List<MenuItemResult> BuildRawTree(List<MenuItem> flatItems)
    {
        // 1. Map Entity sang DTO và đưa vào Dictionary để tra cứu nhanh
        var lookup = flatItems.ToDictionary(x => x.Id, x => new MenuItemResult(
            x.Id.Value,
            x.Label,
            x.Config.Icon,
            x.Config.ActiveIcon,
            x.Config.Path, // Đây là RouterLink
            x.Config.Type,
            x.SortOrder,
            x.Config.CheckRoutes));

        var rootNodes = new List<MenuItemResult>();

        // 2. Duyệt qua từng item để lắp ghép cha con
        foreach (var item in flatItems)
        {
            // Nếu item bị lọc mất ở bước C thì bỏ qua
            if (!lookup.TryGetValue(item.Id, out var dto))
            {
                continue;
            }

            if (item.ParentId is null)
            {
                // Không có cha -> Nó là Root
                rootNodes.Add(dto);
            }
            else
            {
                // Có cha -> Tìm cha trong dictionary
                if (lookup.TryGetValue(item.ParentId, out var parentDto))
                {
                    parentDto.Children.Add(dto);
                }

                // LƯU Ý: Nếu cha nó có tồn tại trong DB, nhưng bị lọc mất ở bước C (do thiếu quyền),
                // thì `lookup` sẽ không chứa cha -> Con trở thành "mồ côi" -> Tự động bị loại bỏ khỏi cây.
                // Điều này đúng với logic: Không thấy cha thì sao thấy con.
            }
        }

        // Sắp xếp lại root
        return rootNodes.OrderBy(x => x.OrderIndex).ToList();
    }

    /// <summary>
    /// Đệ quy cắt tỉa các nhánh cụt (Folder không có con).
    /// </summary>
    private List<MenuItemResult> PruneTreeRecursive(List<MenuItemResult> nodes)
    {
        var keptNodes = new List<MenuItemResult>();

        foreach (var node in nodes)
        {
            // BƯỚC 1: Đệ quy xuống xử lý đám con trước (Depth-First)
            // Đi xuống tận đáy node lá để lọc ngược lên.
            node.Children = PruneTreeRecursive(node.Children);

            // BƯỚC 2: Quyết định số phận của Node hiện tại
            bool shouldKeep = false;

            // Điều kiện A: Nó là Node lá (Có Link thực sự)
            // Dựa vào Config.Path (RouterLink) để xác định.
            if (!string.IsNullOrEmpty(node.RouterLink))
            {
                // Vì nó đã vượt qua "Bộ lọc 1" (Check quyền) nên chắc chắn được giữ.
                shouldKeep = true;
            }

            // Điều kiện B: Nó là Folder (Không có Link) NHƯNG có con cái
            else if (node.Children.Count != 0)
            {
                // Folder cha được giữ lại vì nó chứa ít nhất 1 đứa con hợp lệ.
                shouldKeep = true;
            }

            // Nếu thỏa mãn thì giữ lại
            if (shouldKeep)
            {
                keptNodes.Add(node);
            }
        }

        // Sắp xếp lại thứ tự trước khi trả về
        return keptNodes.OrderBy(n => n.OrderIndex).ToList();
    }
}