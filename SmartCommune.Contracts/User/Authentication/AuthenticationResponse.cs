namespace SmartCommune.Contracts.User.Authentication;

public record AuthenticationResponse(
    Guid UserId,
    string FullName,
    string AccessToken);