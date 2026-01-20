using MediatR;

using Microsoft.AspNetCore.Mvc;

using SmartCommune.Api.Attributes;
using SmartCommune.Application.Common.Constants;
using SmartCommune.Application.Services.Manage.Permissions.Queries.GetAll;

namespace SmartCommune.Api.Controllers.Manage;

[ApiController]
[Route("api/admin/permissions")]
public class PermissionsController(
    ISender sender)
   : BaseController
{
    private readonly ISender _sender = sender;

    [HasPermission(PermissionCodes.Permission.ViewAll)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var query = new GetAllPermissionsQuery();
        var result = await _sender.Send(query, HttpContext.RequestAborted);

        return result.Match(
            Ok,
            HandleProblem);
    }
}