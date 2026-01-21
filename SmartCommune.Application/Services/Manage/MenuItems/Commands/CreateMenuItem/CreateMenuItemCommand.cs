using ErrorOr;

using MediatR;

namespace SmartCommune.Application.Services.Manage.MenuItems.Commands.CreateMenuItem;

public record CreateMenuItemCommand(
    string Label,
    int SortOrder,
    Guid? ParentId,
    string Type,
    string? Path,
    string? Icon,
    string? ActiveIcon,
    List<string> CheckRoutes,
    List<string> RelatedPaths,
    List<Guid> PermissionIds)
    : IRequest<ErrorOr<Guid>>;