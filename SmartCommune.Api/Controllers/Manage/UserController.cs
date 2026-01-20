using MediatR;

using Microsoft.AspNetCore.Mvc;

using SmartCommune.Application.Services.Manage.Users.Commands.CreateUser;
using SmartCommune.Application.Services.Manage.Users.Commands.LockUser;
using SmartCommune.Application.Services.Manage.Users.Commands.UnlockUser;
using SmartCommune.Constracts.Manage.Users;

namespace SmartCommune.Api.Controllers.Manage;

[ApiController]
[Route("api/admin/users")]
public class UserController(
    ISender sender)
   : BaseController
{
    private readonly ISender _sender = sender;

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var command = new CreateUserCommand(
            request.UserName,
            request.Password,
            request.FullName,
            request.RoleId);

        var result = await _sender.Send(command);

        return result.Match(
            userId => CreatedAtAction(nameof(CreateUser), new { id = userId }, userId),
            HandleProblem);
    }

    [HttpPut("{userId:guid}/lock")]
    public async Task<IActionResult> LockUser(Guid userId)
    {
        var command = new LockUserCommand(userId);
        var result = await _sender.Send(command, HttpContext.RequestAborted);

        return result.Match(
            _ => NoContent(),
            HandleProblem);
    }

    [HttpPut("{userId}/unlock")]
    public async Task<IActionResult> UnlockUser(Guid userId)
    {
        var command = new UnlockUserCommand(userId);
        var result = await _sender.Send(command, HttpContext.RequestAborted);

        return result.Match(
            _ => NoContent(),
            HandleProblem);
    }
}