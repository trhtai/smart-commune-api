using ErrorOr;

using MediatR;

using SmartCommune.Application.Services.User.Authentication.Common;

namespace SmartCommune.Application.Services.User.Authentication.Commands.RefreshToken;

public record RefreshTokenCommand(string RefreshToken) : IRequest<ErrorOr<AuthenticationResult>>;