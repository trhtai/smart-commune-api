using ErrorOr;

using MediatR;

using SmartCommune.Application.Services.User.Authentication.Common;

namespace SmartCommune.Application.Services.User.Authentication.Queries.Login;

public record LoginQuery(
    string UserName,
    string Password)
    : IRequest<ErrorOr<AuthenticationResult>>;