using ErrorOr;

using MediatR;

namespace SmartCommune.Application.Services.Manage.Users.Commands.CreateUser;

public record CreateUserCommand(
    string UserName,
    string Password,
    string FullName,
    Guid RoleId)
    : IRequest<ErrorOr<Guid>>;