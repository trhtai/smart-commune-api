namespace SmartCommune.Constracts.User.Authentication;

public record LoginRequest(
    string UserName,
    string Password);