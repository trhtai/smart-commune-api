using MediatR;

using Microsoft.AspNetCore.Mvc;

using SmartCommune.Application.Services.Manage.Users.Commands.LockUser;
using SmartCommune.Application.Services.Manage.Users.Commands.UnlockUser;

namespace SmartCommune.Api.Controllers.Manage;

[ApiController]
[Route("api/admin/users")]
public class UserController(
    ISender sender)
   : BaseController
{
    private readonly ISender _sender = sender;

    [HttpPut("{userId}/lock")]
    public async Task<IActionResult> LockUser(Guid userId)
    {
        var lockUserCommand = new LockUserCommand(userId);
        var result = await _sender.Send(lockUserCommand, HttpContext.RequestAborted);

        return result.Match(
            _ => NoContent(),
            errors => HandleProblem(errors));
    }

    [HttpPut("{userId}/unlock")]
    public async Task<IActionResult> UnlockUser(Guid userId)
    {
        var unlockUserCommand = new UnlockUserCommand(userId);
        var result = await _sender.Send(unlockUserCommand, HttpContext.RequestAborted);

        return result.Match(
            _ => NoContent(),
            errors => HandleProblem(errors));
    }
}