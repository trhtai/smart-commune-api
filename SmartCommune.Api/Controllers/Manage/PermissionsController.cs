using MapsterMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using SmartCommune.Api.Attributes;
using SmartCommune.Application.Common.Constants;
using SmartCommune.Application.Services.Manage.Permissions.Queries.GetAll;
using SmartCommune.Contracts.Manage.Permissions;

namespace SmartCommune.Api.Controllers.Manage;

[ApiController]
[Route("api/admin/permissions")]
public class PermissionsController(
    ISender sender,
    IMapper mapper)
   : BaseController
{
    private readonly ISender _sender = sender;
    private readonly IMapper _mapper = mapper;

    [HasPermission(PermissionCodes.Permission.ViewAll)]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var getAllPermissionsQuery = new GetAllPermissionsQuery();
        var result = await _sender.Send(getAllPermissionsQuery, HttpContext.RequestAborted);

        return result.Match(
            permissions => Ok(_mapper.Map<List<PermissionResponse>>(permissions)),
            errors => HandleProblem(errors));
    }
}