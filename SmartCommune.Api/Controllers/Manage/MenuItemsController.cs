using MapsterMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using SmartCommune.Application.Services.Manage.MenuItems.Commands.UpdateMenuItem;
using SmartCommune.Application.Services.Manage.MenuItems.Queries.GetAll;
using SmartCommune.Constracts.Manage.MenuItems;

namespace SmartCommune.Api.Controllers.Manage;

[ApiController]
[Route("api/admin/menu-items")]
public class MenuItemsController(
    ISender sender,
    IMapper mapper)
    : BaseController
{
    private readonly ISender _sender = sender;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    public async Task<IActionResult> GetMenus()
    {
        var query = new GetAllMenuItemsQuery();
        var result = await _sender.Send(query);

        return result.Match(
            menus => Ok(_mapper.Map<List<MenuItemsResponse>>(menus)),
            HandleProblem);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> UpdateMenuItem(Guid id, UpdateMenuItemRequest request)
    {
        var command = new UpdateMenuItemCommand(
            Id: id,
            Label: request.Title,
            Type: request.Type,
            Path: request.To,
            Icon: request.Icon,
            ActiveIcon: request.ActiveIcon,
            CheckRoutes: request.CheckRoutes ?? [],
            RelatedPaths: request.RelatedPaths ?? [],
            PermissionIds: request.PermissionIds);

        var result = await _sender.Send(command);

        return result.Match(
            _ => NoContent(),
            HandleProblem);
    }
}