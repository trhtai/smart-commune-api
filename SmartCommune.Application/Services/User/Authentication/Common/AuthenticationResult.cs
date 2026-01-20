using SmartCommune.Application.Services.Identity.MenuItems;

namespace SmartCommune.Application.Services.User.Authentication.Common;

public record AuthenticationResult(
    Guid UserId,
    string FullName,
    string AccessToken,
    string RefreshToken,
    List<string> Permissions,
    List<MenuItemResponse> Menu);