namespace SmartCommune.Infrastructure.Security.CurrentUserProvider;

public record CurrentUser(
    Guid Id,
    string FullName,
    Guid RoleId);