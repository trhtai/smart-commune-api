namespace SmartCommune.Application.Services.Identity.MenuItems;

public record MenuItemResult(
    Guid Id,
    string Label,
    string? Icon,
    string? ActiveIcon,
    string? RouterLink,
    string Type,
    int OrderIndex,
    List<string> CheckRoutes)
{
    public List<MenuItemResult> Children { get; set; } = [];
}