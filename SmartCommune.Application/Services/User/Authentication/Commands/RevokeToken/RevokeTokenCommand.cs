using ErrorOr;

using MediatR;

namespace SmartCommune.Application.Services.User.Authentication.Commands.RevokeToken;

public record RevokeTokenCommand(string? RefreshToken) : IRequest<ErrorOr<Success>>;