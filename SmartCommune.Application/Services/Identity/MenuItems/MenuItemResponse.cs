namespace SmartCommune.Application.Services.Identity.MenuItems;

public record MenuItemResponse(
    Guid Id,
    string Label,           // Map từ MenuItem.Label
    string? Icon,           // Map từ MenuItem.Config.Icon
    string? ActiveIcon,     // Map từ MenuItem.Config.ActiveIcon
    string? RouterLink,     // Map từ MenuItem.Config.Path (Chỉ node lá mới có)
    string Type,            // Map từ MenuItem.Config.Type (group, item, submenu...)
    int OrderIndex,         // Map từ MenuItem.SortOrder
    List<string> CheckRoutes) // Map từ MenuItem.Config.CheckRoutes
{
    public List<MenuItemResponse> Children { get; set; } = [];
}