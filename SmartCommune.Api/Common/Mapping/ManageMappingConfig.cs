using Mapster;

using SmartCommune.Application.Services.Manage.Permissions.Common;
using SmartCommune.Application.Services.Manage.Users.Commands.CreateUser;
using SmartCommune.Contracts.Manage.Permissions;
using SmartCommune.Contracts.Manage.Users;

namespace SmartCommune.Api.Common.Mapping;

public class ManageMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Permissions Controller.
        config.NewConfig<PermissionResult, PermissionResponse>();

        // Users Controller.
        config.NewConfig<CreateUserRequest, CreateUserCommand>();
    }
}