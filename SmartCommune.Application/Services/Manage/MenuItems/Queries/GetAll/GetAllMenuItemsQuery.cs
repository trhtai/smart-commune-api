using ErrorOr;

using MediatR;

using SmartCommune.Application.Services.Manage.MenuItems.Common;

namespace SmartCommune.Application.Services.Manage.MenuItems.Queries.GetAll;

public record GetAllMenuItemsQuery : IRequest<ErrorOr<List<MenuItemManageResult>>>;