namespace SmartCommune.Constracts.Manage.Users;

public record CreateUserRequest(
    string UserName,
    string Password,
    string FullName,
    Guid RoleId);