namespace SmartCommune.Application.Services.Identity.MenuItems;

public record MenuItemResult(
    Guid Id,
    string Label,           // Map từ MenuItem.Label
    string? Icon,           // Map từ MenuItem.Config.Icon
    string? ActiveIcon,     // Map từ MenuItem.Config.ActiveIcon
    string? RouterLink,     // Map từ MenuItem.Config.Path (Chỉ node lá mới có)
    string Type,            // Map từ MenuItem.Config.Type (group, item, submenu...)
    int OrderIndex,         // Map từ MenuItem.SortOrder
    List<string> CheckRoutes) // Map từ MenuItem.Config.CheckRoutes
{
    public List<MenuItemResult> Children { get; set; } = [];
}