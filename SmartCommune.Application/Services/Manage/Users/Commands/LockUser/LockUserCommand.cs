using ErrorOr;

using MediatR;

namespace SmartCommune.Application.Services.Manage.Users.Commands.LockUser;

public record LockUserCommand(Guid UserId) : IRequest<ErrorOr<Unit>>;