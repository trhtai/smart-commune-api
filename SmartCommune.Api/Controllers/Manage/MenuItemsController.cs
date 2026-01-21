using MediatR;

using Microsoft.AspNetCore.Mvc;

using SmartCommune.Application.Services.Manage.MenuItems.Commands.CreateMenuItem;
using SmartCommune.Application.Services.Manage.MenuItems.Queries.GetAll;
using SmartCommune.Constracts.Manage.MenuItems;

namespace SmartCommune.Api.Controllers.Manage;

[ApiController]
[Route("api/admin/menu-items")]
public class MenuItemsController(
    ISender sender)
    : BaseController
{
    private readonly ISender _sender = sender;

    [HttpGet]
    public async Task<IActionResult> GetMenus()
    {
        var result = await _sender.Send(new GetAllMenuItemsQuery());
        return result.Match(Ok, HandleProblem);
    }

    [HttpPost]
    public async Task<IActionResult> CreateMenuItem(CreateMenuItemRequest request)
    {
        var command = new CreateMenuItemCommand(
            Label: request.Label,
            SortOrder: request.SortOrder,
            ParentId: request.ParentId,
            Type: request.Type,
            Path: request.Path,
            Icon: request.Icon,
            ActiveIcon: request.ActiveIcon,
            CheckRoutes: request.CheckRoutes ?? [],
            RelatedPaths: request.RelatedPaths ?? [],
            PermissionIds: request.PermissionIds);

        var result = await _sender.Send(command);

        return result.Match(
            menuItemId => CreatedAtAction(
                nameof(GetMenuItem),
                new { id = menuItemId },
                new { Id = menuItemId }),
            HandleProblem);
    }

    [HttpGet("{id:guid}")]
    public IActionResult GetMenuItem(Guid id)
    {
        return Ok(); // Implement logic lấy chi tiết sau
    }
}