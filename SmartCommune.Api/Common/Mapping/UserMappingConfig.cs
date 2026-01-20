using Mapster;

using SmartCommune.Application.Services.User.Authentication.Common;
using SmartCommune.Application.Services.User.Authentication.Queries.Login;
using SmartCommune.Contracts.User.Authentication;

namespace SmartCommune.Api.Common.Mapping;

public class UserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Authentication Controller.
        config.NewConfig<LoginRequest, LoginQuery>();
        config.NewConfig<AuthenticationResult, AuthenticationResponse>();
    }
}