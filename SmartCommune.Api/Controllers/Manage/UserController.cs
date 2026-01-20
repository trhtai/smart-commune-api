using MapsterMapper;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using SmartCommune.Application.Services.Manage.Users.Commands.CreateUser;
using SmartCommune.Application.Services.Manage.Users.Commands.LockUser;
using SmartCommune.Application.Services.Manage.Users.Commands.UnlockUser;
using SmartCommune.Contracts.Manage.Users;

namespace SmartCommune.Api.Controllers.Manage;

[ApiController]
[Route("api/admin/users")]
public class UserController(
    ISender sender,
    IMapper mapper)
   : BaseController
{
    private readonly ISender _sender = sender;
    private readonly IMapper _mapper = mapper;

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        var createUserCommand = _mapper.Map<CreateUserCommand>(request);
        var result = await _sender.Send(createUserCommand);

        return result.Match(
            userId => CreatedAtAction(nameof(CreateUser), new { id = userId }, userId),
            errors => HandleProblem(errors));
    }

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