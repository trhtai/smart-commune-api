using ErrorOr;

using MediatR;

namespace SmartCommune.Application.Services.Manage.MenuItems.Commands.UpdateMenuItem;

public record UpdateMenuItemCommand(
    Guid Id,
    string Label,
    string Type,
    string? Path,
    string? Icon,
    string? ActiveIcon,
    List<string> CheckRoutes,
    List<string> RelatedPaths,
    List<Guid> PermissionIds)
    : IRequest<ErrorOr<Updated>>;