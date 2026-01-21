namespace SmartCommune.Constracts.User.Authentication;

public record AuthenticationResponse(
    Guid UserId,
    string FullName,
    string AccessToken,
    List<string> Permissions);
    //List<MenuItemResponse> Menu);