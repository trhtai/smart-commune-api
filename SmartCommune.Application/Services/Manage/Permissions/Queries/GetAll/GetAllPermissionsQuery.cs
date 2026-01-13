using ErrorOr;

using MediatR;

using SmartCommune.Application.Services.Manage.Permissions.Common;

namespace SmartCommune.Application.Services.Manage.Permissions.Queries.GetAll;

public record GetAllPermissionsQuery : IRequest<ErrorOr<List<PermissionResult>>>;