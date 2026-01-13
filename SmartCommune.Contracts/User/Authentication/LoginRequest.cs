namespace SmartCommune.Contracts.User.Authentication;

public record LoginRequest(
    string UserName,
    string Password);