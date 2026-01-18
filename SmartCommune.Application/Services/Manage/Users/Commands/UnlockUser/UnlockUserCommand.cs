using ErrorOr;

using MediatR;

namespace SmartCommune.Application.Services.Manage.Users.Commands.UnlockUser;

public record UnlockUserCommand(Guid UserId) : IRequest<ErrorOr<Unit>>;